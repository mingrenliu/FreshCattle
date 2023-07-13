using InheritAnalyzer.TransformInfo;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static InheritAnalyzer.TextParseFactory;

namespace InheritAnalyzer
{
    internal class ClassInfoProvider
    {
        public static ClassInfoProvider Create(string path)
        {
            var provider = new FileProvider(path);
            var result = new ClassInfoProvider(provider);
            provider.SetProvider(result);
            return result;
        }

        private readonly ConcurrentDictionary<string, ClassInfo> _info = new();
        private readonly ConcurrentDictionary<string, IEnumerable<string>> _fileMapToInfo = new();

        private readonly FileProvider _fileProvider;

        private ClassInfoProvider(FileProvider provider)
        {
            _fileProvider = provider;
        }

        public bool TryGetClassInfo(string name, out ClassInfo info)
        {
            return _info.TryGetValue(name, out info);
        }

        public void Delete(string path)
        {
            if (_fileMapToInfo.TryRemove(path, out var values))
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
                _fileMapToInfo[path] = info.Select(x => x.ClassName);
                foreach (var item in info)
                {
                    _info[item.ClassName] = item;
                }
            }
        }

        public void Rename(string path, string newPath)
        {
            if (_fileMapToInfo.TryRemove(path, out var value))
            {
                _fileMapToInfo[newPath] = value;
            }
        }

        public async Task Changed(string path)
        {
            Delete(path);
            await Add(path);
        }

        private class FileProvider : IDisposable
        {
            private readonly FileSystemWatcher _watch;
            private ClassInfoProvider _infoProvider;
            internal string Path { get; }
            private readonly Dictionary<string, AdditionalText> _additionText;

            public void SetProvider(ClassInfoProvider provider)
            {
                _infoProvider = provider;
            }

            public FileProvider(string path)
            {
                Path = path;
                Init();
                var watch = new FileSystemWatcher(Path, "*.cs") { IncludeSubdirectories = false, EnableRaisingEvents = true };
                watch.Deleted += (sender, args) => DeletedAction(sender, args);
                watch.Renamed += (sender, args) => RenamedAction(sender, args);
                watch.Created += (sender, args) => CreatedAction(sender, args);
                watch.Changed += (sender, args) => ChangedAction(sender, args);
                _watch = watch;
            }

            private void Init()
            {
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
                var exist = _additionText.TryGetValue(e.FullPath, out var value);
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
                _watch.Dispose();
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
                var str = SourceText.From(File.ReadAllText(Path));
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