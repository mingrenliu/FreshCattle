using InheritAnalyzer.TransformInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static InheritAnalyzer.TextParseFactory;

namespace InheritAnalyzer
{
    public class ClassInfoProvider
    {
        public static ClassInfoProvider Create(string path,bool includeSubdir)
        {
            var provider= new ClassInfoProvider(path,includeSubdir);
            provider.StartWatch();
            return provider;
        }
        public IEnumerable<ClassInfo> Values=>_info.Values;
        private readonly ConcurrentDictionary<ClassSimpleInfo, ClassInfo> _info = new();
        private readonly ConcurrentDictionary<string, IEnumerable<ClassSimpleInfo>> _fileMapToClassInfo = new();

        private readonly FileProvider _fileProvider;
        private string Path { get; }
        private ClassInfoProvider(string path,bool includeSubdir)
        {
            Path = path;
            _fileProvider = new FileProvider(this,includeSubdir);
            Load();
        }
        private async void Load()
        {
            foreach (var text in _fileProvider.GetAllText())
            {
                var info =await ParseText(text);
                if (info.Any())
                {
                    foreach (var item in info)
                    {
                        _info[new(item.ClassName,item.TypeParameterCount)] = item;
                    }
                    _fileMapToClassInfo[text.Path] = info.Select(x => new ClassSimpleInfo(x.ClassName,x.TypeParameterCount));
                }
            }
        }
        public void StartWatch() => _fileProvider.StartWatch();
        public bool TryGetClassInfo(ClassSimpleInfo simpleInfo, out ClassInfo info)
        {
            return _info.TryGetValue(simpleInfo, out info);
        }

        public void Delete(string path)
        {
            if (_fileMapToClassInfo.TryRemove(path, out var values))
            {
                foreach (var item in values)
                {
                    _info.TryRemove(item, out _);
                }
            }
        }

        public async Task Add(string path)
        {
            var info = await ParseText(_fileProvider.GetText(path));
            if (info.Any())
            {
                foreach (var item in info)
                {
                    _info[new ClassSimpleInfo(item.ClassName,item.TypeParameterCount)] = item;
                }
                _fileMapToClassInfo[path] = info.Select(x => new ClassSimpleInfo(x.ClassName,x.TypeParameterCount));
            }
        }

        public void Rename(string path, string newPath)
        {
            if (_fileMapToClassInfo.TryRemove(path, out var value))
            {
                _fileMapToClassInfo[newPath] = value;
            }
        }

        public async Task Changed(string path)
        {
            Delete(path);
            await Add(path);
        }

        private class FileProvider : IDisposable
        {
            private IDisposable _disposable;
            private ClassInfoProvider _infoProvider;
            internal string Path =>_infoProvider.Path;
            private bool _includeSubdir;
            private readonly Dictionary<string, AdditionalText> _additionText=new();
            public IEnumerable<AdditionalText> GetAllText()
            {
                return _additionText.Values;
            }
            public void SetProvider(ClassInfoProvider provider)
            {
                _infoProvider = provider;
            }

            public FileProvider(ClassInfoProvider provider,bool includeSubdir)
            {
                _infoProvider= provider;
                _includeSubdir= includeSubdir;
                Init();

            }
            public void StartWatch()
            {
                var watch = new FileSystemWatcher(Path, "*.cs") { IncludeSubdirectories = _includeSubdir, EnableRaisingEvents = true };
                watch.Deleted += (sender, args) => DeletedAction(sender, args);
                watch.Renamed += (sender, args) => RenamedAction(sender, args);
                watch.Created += (sender, args) => CreatedAction(sender, args);
                watch.Changed += (sender, args) => ChangedAction(sender, args);
                _disposable = watch;
            }
            private void Init()
            {
                if (!Directory.Exists(Path))
                {
                    return;
                }
                var fileInfo = Directory.GetFiles(Path, "*.cs", SearchOption.TopDirectoryOnly);
                foreach (var item in fileInfo)
                {
                    _additionText[item] = new LocalFileAdditionalText(item);
                }
            }

            public AdditionalText GetText(string path)
            {
                return _additionText.TryGetValue(path, out var value) ? value : null;
            }

            private void DeletedAction(object sender, FileSystemEventArgs e)
            {
                _additionText.Remove(e.FullPath);
                _infoProvider.Delete(e.FullPath);
            }

            private void RenamedAction(object sender, RenamedEventArgs e)
            {
                var exist = _additionText.TryGetValue(e.FullPath, out _);
                if (exist)
                {
                    _additionText.Remove(e.OldFullPath);
                    _additionText[e.FullPath] = new LocalFileAdditionalText(e.FullPath);
                    _infoProvider.Rename(e.OldFullPath, e.FullPath);
                }
            }

            private async void ChangedAction(object sender, FileSystemEventArgs e)
            {
                _additionText[e.FullPath] = new LocalFileAdditionalText(e.FullPath);
                await _infoProvider.Changed(e.FullPath);
            }

            private async void CreatedAction(object sender, FileSystemEventArgs e)
            {
                _additionText.Add(e.FullPath, new LocalFileAdditionalText(e.FullPath));
                await _infoProvider.Changed(e.FullPath);
            }

            public void Dispose()
            {
                _disposable?.Dispose();
            }
        }
    }

    internal class LocalFileAdditionalText : AdditionalText
    {
        public override string Path { get; }
        private bool haveInit;
        private SourceText _text;

        public LocalFileAdditionalText(string path)
        {
            Path = path;
            Load();
        }

        private void Load()
        {
            if (File.Exists(Path))
            {
                var str = SourceText.From(File.ReadAllText(Path, UTF8Encoding.Default), UTF8Encoding.Default);
                Interlocked.Exchange(ref _text, str);
            }
            haveInit = true;
        }

        public override SourceText GetText(CancellationToken cancellationToken = default)
        {
            if (!haveInit)
            {
                Load();
            }
            return _text;
        }
    }
}