using MethodInvoker = System.Windows.Forms.MethodInvoker;

namespace Il2CppDumper.WinForms.Forms;

public partial class FrmMain : Form
{
    #region Variable

    private readonly string applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
    private readonly string tempPath = Path.GetTempPath() + "\\";
    private static Config _config = new();
    private static string? _binaryPath, _metadataPath, _outputDir;

    private string? BinaryPath
    {
        get => _binaryPath;
        set => SetAndUpdate(ref _binaryPath, value, txtBinaryPath);
    }

    private string? MetadataPath
    {
        get => _metadataPath;
        set => SetAndUpdate(ref _metadataPath, value, txtMetadataPath);
    }

    private string? OutputDir
    {
        get => _outputDir;
        set => SetAndUpdate(ref _outputDir, value, txtOutputDir);
    }

    #endregion Variable

    public FrmMain(string[]? args = null)
    {
        InitializeComponent();
        if (args is { Length: > 0 })
        {
            RunDump(args);
        }
    }

    #region Load/Save

    private void FrmMain_Load(object sender, EventArgs e)
    {
#pragma warning disable SYSLIB0014
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#pragma warning restore SYSLIB0014

        Text += $@" - {Assembly.GetExecutingAssembly().GetName().Version}";
        LoadLocation();
        cbArch.SelectedIndex = Settings.Default.Arch;
        Logger.LogCallback = WriteOutputInfo;

        if (File.Exists("config.json"))
        {
            var config = JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
            if (config is not null)
            {
                _config = config;
                return;
            }

            WriteOutputError("Invalid \"config.json\"");
        }
        else
        {
            WriteOutputWarning("Not found \"config.json\"");
        }

        var json = JsonSerializer.Serialize(new Config());

        WriteOutputInfo("Create new \"config.json\"");
        File.WriteAllText("config.json", json);

        _config = new Config();
    }

    private void LoadLocation()
    {
        if (Settings.Default.Location == new Point(0, 0))
        {
            CenterToScreen();
        }
        else
        {
            Location = Settings.Default.Location;
        }
    }

    private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        Settings.Default.Arch = cbArch.SelectedIndex;
        CloseLocation();
    }

    private void CloseLocation()
    {
        Settings.Default.Location = Location;
        Settings.Default.Save();
    }

    #endregion Load/Save

    #region Button EventArgs

    private void btnSelectBinary_Click(object sender, EventArgs e)
    {
        if (openBin.ShowDialog() != DialogResult.OK) return;

        BinaryPath = openBin.FileName;
        txtCodeRegis.Clear();
        txtMetaRegis.Clear();

        if (Settings.Default.AutoSetDir)
        {
            OutputDir = Path.GetDirectoryName(BinaryPath) + @"\dumped\";
        }
    }

    private void btnSelectMetadata_Click(object sender, EventArgs e)
    {
        if (openDat.ShowDialog() != DialogResult.OK) return;

        MetadataPath = openDat.FileName;
        txtCodeRegis.Clear();
        txtMetaRegis.Clear();
    }

    private void btnSelectDir_Click(object sender, EventArgs e)
    {
        if (openDir.ShowDialog() != DialogResult.OK) return;

        OutputDir = openDir.SelectedPath + @"\";
    }

    private void btnOpenDir_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(OutputDir)) return;

        Process.Start("explorer.exe", OutputDir);
    }

    private void btnSettings_Click(object sender, EventArgs e)
    {
        var frmSettings = new FrmSettings();
        {
            frmSettings.Location = new Point(Location.X + 150, Location.Y + 120);
            frmSettings.ShowDialog();
            frmSettings.Dispose();
        }
    }

    private async void btnDump_ClickAsync(object sender, EventArgs e)
    {
        try
        {
            rbLog.Clear();

            if (string.IsNullOrWhiteSpace(BinaryPath))
            {
                WriteOutputWarning("Executable file is not selected");
                return;
            }

            if (string.IsNullOrWhiteSpace(MetadataPath))
            {
                WriteOutputWarning("Metadata-global.dat file is not selected");
                return;
            }

            if (string.IsNullOrWhiteSpace(OutputDir))
            {
                WriteOutputWarning("Output Directory can not be empty");
                return;
            }

            if (!Directory.Exists(OutputDir))
            {
                WriteOutputWarning("Output directory does not exist");
                try
                {
                    Directory.CreateDirectory($"{OutputDir}");
                    WriteOutputInfo($"Create directory at {OutputDir}");
                }
                catch (Exception ex)
                {
                    WriteOutputError("Can not create directory: " + ex.Message);
                    return;
                }
            }

            FormState(AppStatus.Running);

            await Task.Factory.StartNew(() => Dumper(BinaryPath, MetadataPath, OutputDir)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            WriteOutputError("Dump Click: " + ex.Message);
        }
        finally
        {
            FormState(AppStatus.Idle);
        }
    }

    #endregion Button EventArgs

    #region Dump

    private void Dumper(string binaryPath, string metadataPath, string outputPath)
    {
        try
        {
            EnsureDirectoryForFile(outputPath);
            if (!Init(binaryPath, metadataPath, out var metadata, out var il2Cpp) || il2Cpp == null) return;

            Dump(metadata, il2Cpp, outputPath);
            CopyScripts(outputPath);
        }
        catch (Exception ex)
        {
            WriteOutputError($"Dumper {ex.Message}");
        }
    }

    private bool Init(string il2CppPath, string metadataPath, out Metadata metadata, out Il2Cpp? il2Cpp)
    {
        metadata = InitializingMetadata(metadataPath);
        il2Cpp = InitializingIl2Cpp(il2CppPath);
        if (il2Cpp is null) return false;

        var version = _config.ForceIl2CppVersion ? _config.ForceVersion : metadata.Version;
        il2Cpp.SetProperties(version, metadata.metadataUsagesCount);
        WriteOutputInfo($"Il2Cpp Version: {il2Cpp.Version}");
        if (_config.ForceDump || il2Cpp.CheckDump())
        {
            if (il2Cpp is ElfBase elf)
            {
                var value = "";
                if (InputBox.Show("Input il2cpp dump address or input 0 to force continue:", "Detected this may be a dump file", ref value) != DialogResult.OK) return false;
                var dumpAddress = Convert.ToUInt64(value, 16);
                if (dumpAddress != 0)
                {
                    WriteOutputInfo($"il2cpp dump address: {dumpAddress}");
                    il2Cpp.ImageBase = dumpAddress;
                    il2Cpp.IsDumped = true;
                    if (!_config.NoRedirectedPointer)
                    {
                        elf.Reload();
                    }
                }
            }
            else
            {
                il2Cpp.IsDumped = true;
            }
        }

        WriteOutputInfo("Searching...");
        try
        {
            var flag = il2Cpp.PlusSearch(metadata.methodDefs.Count(x => x.methodIndex >= 0), metadata.typeDefs.Length, metadata.imageDefs.Length);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !flag && il2Cpp is PE)
            {
                WriteOutputInfo("Use custom PE loader");
                il2Cpp = PELoader.Load(il2CppPath);
                il2Cpp.SetProperties(version, metadata.metadataUsagesCount);
                flag = il2Cpp.PlusSearch(metadata.methodDefs.Count(x => x.methodIndex >= 0), metadata.typeDefs.Length, metadata.imageDefs.Length);
            }
            if (!flag)
            {
                flag = il2Cpp.Search();
            }
            if (!flag)
            {
                flag = il2Cpp.SymbolSearch();
            }
            if (!flag)
            {
                WriteOutputError("ERROR: Can't use auto mode to process file, try manual mode.");

                ulong codeRegistration;
                if (string.IsNullOrWhiteSpace(txtCodeRegis.Text))
                {
                    var codeValue = "";
                    if (InputBox.Show(@"Input CodeRegistration: ", "", ref codeValue) != DialogResult.OK) return false;
                    codeRegistration = Convert.ToUInt64(codeValue, 16);
                    WriteOutputInfo($"CodeRegistration: {codeValue}");
                }
                else
                {
                    codeRegistration = Convert.ToUInt64(txtCodeRegis.Text, 16);
                }

                ulong metadataRegistration;
                if (string.IsNullOrWhiteSpace(txtMetaRegis.Text))
                {
                    var metadataValue = "";
                    if (InputBox.Show("Input MetadataRegistration: ", "", ref metadataValue) != DialogResult.OK) return false;
                    metadataRegistration = Convert.ToUInt64(metadataValue, 16);
                    WriteOutputInfo($"MetadataRegistration: {metadataValue}");
                }
                else
                {
                    metadataRegistration = Convert.ToUInt64(txtMetaRegis.Text, 16);
                }

                il2Cpp.Init(codeRegistration, metadataRegistration);
            }

            if (il2Cpp is { Version: >= 27, IsDumped: true })
            {
                var typeDef = metadata.typeDefs[0];
                var il2CppType = il2Cpp.types[typeDef.byvalTypeIndex];
                metadata.ImageBase = il2CppType.data.typeHandle - metadata.header.typeDefinitionsOffset;
            }
        }
        catch (Exception e)
        {
            WriteOutputError("Init: " + e.Message);
            WriteOutputError("ERROR: An error occurred while processing.");
            return false;
        }
        return true;
    }

    private void Dump(Metadata metadata, Il2Cpp il2Cpp, string outputDir)
    {
        WriteOutputInfo("Dumping...");
        var executor = new Il2CppExecutor(metadata, il2Cpp);
        var decompiler = new Il2CppDecompiler(executor);
        decompiler.Decompile(_config, outputDir);
        WriteOutputSucceed("Done!");
        if (_config.GenerateStruct)
        {
            WriteOutputInfo("Generate struct...");
            var scriptGenerator = new StructGenerator(executor);
            scriptGenerator.WriteScript(outputDir);
            WriteOutputSucceed("Done!");
        }
        if (_config.GenerateDummyDll)
        {
            WriteOutputInfo("Generate dummy dll...");
            DummyAssemblyExporter.Export(executor, outputDir, _config.DummyDllAddToken);
            WriteOutputSucceed("Done!");
            Directory.SetCurrentDirectory(applicationPath); //Fix read-only directory permission
        }
    }

    private void CopyScripts(string outputPath)
    {
        var guiPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
        if (Settings.Default.OutputScripts == null) return;
        var scripts = Settings.Default.OutputScripts.Cast<string>().ToList();
        if (scripts.Count == 0) return;

        foreach (var script in scripts)
        {
            var source = guiPath + script;
            var dest = Path.Combine(outputPath, script);
            if (!File.Exists(source) || File.Exists(dest)) continue;

            WriteOutputWarning($"Does not exist \"{script}\" in the dump directory, copying...");

            try
            {
                File.Copy(source, dest);
                WriteOutputSucceed($"Create \"{script}\" at {dest}");
            }
            catch
            {
                WriteOutputError($"Can not create \"{script}\"");
                return;
            }
        }
    }

    private Metadata InitializingMetadata(string metadataPath)
    {
        WriteOutputInfo("Initializing metadata...");
        var metadataBytes = File.ReadAllBytes(metadataPath);
        var metadata = new Metadata(new MemoryStream(metadataBytes));
        WriteOutputInfo($"Metadata Version: {metadata.Version}");

        return metadata;
    }

    private Il2Cpp? InitializingIl2Cpp(string il2CppPath)
    {
        WriteOutputInfo("Initializing il2cpp file...");
        var il2CppBytes = File.ReadAllBytes(il2CppPath);
        var il2CppMagic = BitConverter.ToUInt32(il2CppBytes, 0);
        var il2CppMemory = new MemoryStream(il2CppBytes);
        switch (il2CppMagic)
        {
            default:
                {
                    WriteOutputError("ERROR: il2cpp file not supported.");
                    return null;
                }

            case 0x6D736100:
                {
                    var web = new WebAssembly(il2CppMemory);
                    return web.CreateMemory();
                }

            case 0x304F534E:
                {
                    var nso = new NSO(il2CppMemory);
                    return nso.UnCompress();
                }

            case 0x905A4D: //PE
                {
                    return new PE(il2CppMemory);
                }

            case 0x464c457f: //ELF
                {
                    if (il2CppBytes[4] == 2) //ELF64
                    {
                        return new Elf64(il2CppMemory);
                    }
                    else
                    {
                        return new Elf(il2CppMemory);
                    }
                }

            case 0xCAFEBABE: //FAT Mach-O
            case 0xBEBAFECA:
                {
                    var machoFat = new MachoFat(new MemoryStream(il2CppBytes));
                    var fatMagic = "";
                    for (var i = 0; i < machoFat.fats.Length; i++)
                    {
                        var fat = machoFat.fats[i];
                        fatMagic += fat.magic == 0xFEEDFACF ? $"{i + 1}.64bit\n" : $"{i + 1}.32bit\n";
                    }
                    var key = "";
                    if (InputBox.Show(fatMagic, "Select Platform:", ref key) != DialogResult.OK)
                    {
                        return null;
                    }
                    var index = int.Parse(key) - 1;
                    var magic = machoFat.fats[index % 2].magic;
                    il2CppBytes = machoFat.GetMacho(index % 2);
                    il2CppMemory = new MemoryStream(il2CppBytes);

                    if (magic == 0xFEEDFACF)
                        goto case 0xFEEDFACF;

                    goto case 0xFEEDFACE;
                }
            case 0xFEEDFACF: // 64bit Mach-O
                {
                    return new Macho64(il2CppMemory);
                }

            case 0xFEEDFACE: // 32bit Mach-O
                {
                    return new Macho(il2CppMemory);
                }
        }
    }

    #endregion Dump

    #region Drag/Drop

    private async void RunDump(string[] dragFiles)
    {
        try
        {
            FormState(AppStatus.Running);

            if (dragFiles.Length > 1)
            {
                DeleteExistsFile(tempPath + "global-metadata.dat");
                DeleteExistsFile(tempPath + "libil2cpp.so");
            }

            var directoryPath = Path.GetDirectoryName(dragFiles[0]);
            var fileName = Path.GetFileNameWithoutExtension(dragFiles[0]);
            if (string.IsNullOrWhiteSpace(directoryPath) || string.IsNullOrWhiteSpace(fileName))
            {
                WriteOutputError("RunDump: Invalid file path");
                return;
            }

            var outputPath = Path.Combine(directoryPath, fileName + "_dumped");
            if (Settings.Default.AutoSetDir)
            {
                OutputDir = outputPath;
            }

            foreach (var file in dragFiles)
            {
                switch (Path.GetExtension(file))
                {
                    case ".so":
                        BinaryPath = file;
                        break;

                    case ".dat":
                        MetadataPath = file;
                        break;

                    case ".apk":
                        {
                            rbLog.Text = "";
                            if (dragFiles.Length > 1)
                            {
                                WriteOutputInfo("Dumping Il2Cpp from split APKs...");
                                await ApkSplitDumping(file, outputPath);
                            }
                            else
                            {
                                await ApkDumping(file, outputPath);
                            }

                            break;
                        }
                    case ".apks":
                    case ".xapk":
                        rbLog.Text = "";
                        await ApksDumping(file, outputPath);
                        break;

                    case ".ipa":
                        rbLog.Text = "";
                        await IosDumping(file, outputPath);
                        break;

                    default:
                        BinaryPath = file;
                        break;
                }

                if (Settings.Default.AutoSetDir)
                {
                    OutputDir = Path.GetDirectoryName(file) + @"\dumped\";
                }
            }
        }
        catch (Exception ex)
        {
            WriteOutputError($"Run Dump: {ex.Message}");
        }
        finally
        {
            FormState(AppStatus.Idle);
        }
    }

    private async void FrmMain_DragDropAsync(object sender, DragEventArgs e)
    {
        try
        {
            FormState(AppStatus.Running);

            if (e.Data == null) return;
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
            if (files == null) return;

            if (files.Length > 1)
            {
                DeleteExistsFile(tempPath + "global-metadata.dat");
                DeleteExistsFile(tempPath + "libil2cpp.so");
            }

            var outputPath = Path.GetDirectoryName(files[0]) + "\\" + Path.GetFileNameWithoutExtension(files[0]) + "_dumped\\";
            if (Settings.Default.AutoSetDir)
            {
                OutputDir = outputPath;
            }

            foreach (var file in files)
            {
                switch (Path.GetExtension(file))
                {
                    case ".so":
                        BinaryPath = file;
                        break;

                    case ".dat":
                        MetadataPath = file;
                        break;

                    case ".apk":
                        {
                            rbLog.Text = "";
                            if (files.Length > 1)
                            {
                                WriteOutputInfo("Dumping Il2Cpp from split APKs...");
                                await ApkSplitDumping(file, outputPath).ConfigureAwait(false);
                            }
                            else
                            {
                                await ApkDumping(file, outputPath).ConfigureAwait(false);
                            }

                            break;
                        }
                    case ".apks":
                    case ".xapk":
                        rbLog.Text = "";
                        await ApksDumping(file, outputPath).ConfigureAwait(false);
                        break;

                    case ".ipa":
                        rbLog.Text = "";
                        await IosDumping(file, outputPath).ConfigureAwait(false);
                        break;

                    default:
                        BinaryPath = file;
                        break;
                }
            }
            FormState(AppStatus.Idle);
        }
        catch (Exception ex)
        {
            WriteOutputError($"DragDropAsync: {ex.Message}");
        }
    }

    private void FrmMain_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data != null && !e.Data.GetDataPresent(DataFormats.FileDrop)) return;
        e.Effect = DragDropEffects.Copy;
    }

    #endregion Drag/Drop

    #region Auto Dump

    private Task IosDumping(string file, string outputPath)
    {
        return Task.Factory.StartNew(() =>
        {
            using var archive = ZipFile.OpenRead(file);
            var ipaBinaryFolder = archive.Entries.FirstOrDefault(f => f.FullName.StartsWith("Payload/") && f.FullName.Contains(".app/") && f.FullName.Count(x => x == '/') == 2);

            if (ipaBinaryFolder == null)
            {
                WriteOutputError("Failed to extract required file. Please extract the files manually");
                return;
            }

            var myRegex3 = new Regex(@"(?<=Payload\/)(.*?)(?=.app\/)", RegexOptions.None);
            var match = myRegex3.Match(ipaBinaryFolder.FullName);

            var ipaBinaryName = match.ToString();
            var metadataFile = archive.Entries.FirstOrDefault(f =>
                f.FullName == $"Payload/{ipaBinaryName}.app/Data/Managed/Metadata/global-metadata.dat");
            var binaryFile = archive.Entries.FirstOrDefault(f =>
                                 f.FullName ==
                                 $"Payload/{ipaBinaryName}.app/Frameworks/UnityFramework.framework/UnityFramework") ??
                             archive.Entries.FirstOrDefault(f =>
                                 f.FullName == $"Payload/{ipaBinaryName}.app/{ipaBinaryName}");

            if (metadataFile == null || binaryFile == null)
            {
                WriteOutputError("This IPA does not contain an IL2CPP application");
                return;
            }

            WriteOutputInfo("Extract global-metadata.dat to temp path");
            metadataFile.ExtractToFile(tempPath + "global-metadata.dat", true);
            if (Settings.Default.ExtractMetaData)
            {
                WriteOutputInfo("Extract global-metadata.dat to dump path");
                metadataFile.ExtractToFile(EnsureDirectoryForFile(outputPath + "\\global-metadata.dat"), true);
            }

            if (Settings.Default.IsMachO64)
            {
                WriteOutputInfo("Dumping ARM64...");

                WriteOutputInfo("Extract binary file to temp path");
                binaryFile.ExtractToFile(tempPath + "arm64", true);

                if (Settings.Default.ExtractBinary)
                {
                    WriteOutputInfo("Extract binary file to dump path");
                    binaryFile.ExtractToFile(EnsureDirectoryForFile(outputPath + $"/{binaryFile.Name}"), true);
                }

                Dumper(tempPath + "arm64", tempPath + "global-metadata.dat", EnsureDirectoryForFile(outputPath + "\\"));
            }
            else
            {
                WriteOutputInfo("Dumping ARMv7...");

                WriteOutputInfo("Extract binary file to temp path");
                binaryFile.ExtractToFile(tempPath + "armv7", true);

                if (Settings.Default.ExtractBinary)
                {
                    WriteOutputInfo("Extract binary file to dump path");
                    binaryFile.ExtractToFile(EnsureDirectoryForFile(outputPath + $"/{binaryFile.Name}"), true);
                }

                Dumper(tempPath + "armv7", tempPath + "global-metadata.dat", EnsureDirectoryForFile(outputPath + "\\"));
            }
        });
    }

    private Task ApkDumping(string file, string outputPath)
    {
        return Task.Factory.StartNew(() =>
        {
            using var archive = ZipFile.OpenRead(file);
            var binaryFile = archive.Entries.FirstOrDefault(f => f.Name.Contains("libil2cpp.so"));
            var metadataFile = archive.Entries.FirstOrDefault(f => f.FullName == "assets/bin/Data/Managed/Metadata/global-metadata.dat");

            if (binaryFile == null)
            {
                WriteOutputWarning("This APK does not contain libil2cpp.so file");
                return;
            }

            if (metadataFile == null)
            {
                WriteOutputWarning("This APK contains il2cpp but does not contain global-metadata.dat. It may be protected or APK has been split");
                return;
            }

            WriteOutputInfo("Extract global-metadata.dat to temp path");
            metadataFile.ExtractToFile(tempPath + "global-metadata.dat", true);

            if (Settings.Default.ExtractMetaData)
            {
                WriteOutputInfo("Extract global-metadata.dat to dump path");
                metadataFile.ExtractToFile(EnsureDirectoryForFile(outputPath + "\\global-metadata.dat"), true);
            }

            foreach (var entry in archive.Entries)
            {
                if (entry.FullName.Equals("lib/armeabi-v7a/libil2cpp.so") && cbArch.SelectedIndex is 0 or 1)
                {
                    WriteOutputInfo("Dumping ARMv7...");

                    WriteOutputInfo("Extract binary file to temp path");
                    entry.ExtractToFile(tempPath + "libil2cpparmv7", true);

                    if (Settings.Default.ExtractBinary)
                    {
                        WriteOutputInfo("Extract binary file to dump path");
                        entry.ExtractToFile(EnsureDirectoryForFile(outputPath + "\\ARMv7\\libil2cpp.so"), true);
                    }

                    Dumper(tempPath + "libil2cpparmv7", tempPath + "global-metadata.dat", EnsureDirectoryForFile(outputPath + "\\ARMv7\\"));
                }

                if (entry.FullName.Equals("lib/arm64-v8a/libil2cpp.so") && cbArch.SelectedIndex is 0 or 2)
                {
                    WriteOutputInfo("Dumping ARM64...");

                    WriteOutputInfo("Extract binary file to temp path");
                    entry.ExtractToFile(tempPath + "libil2cpparm64", true);

                    if (Settings.Default.ExtractBinary)
                    {
                        WriteOutputInfo("Extract binary file to dump path");
                        entry.ExtractToFile(EnsureDirectoryForFile(outputPath + "\\ARM64\\libil2cpp.so"), true);
                    }

                    Dumper(tempPath + "libil2cpparm64", tempPath + "global-metadata.dat", EnsureDirectoryForFile(outputPath + "\\ARM64\\"));
                }

                if (entry.FullName.Equals("lib/x86/libil2cpp.so") && cbArch.SelectedIndex is 0)
                {
                    WriteOutputInfo("Dumping x86...");

                    WriteOutputInfo("Extract binary file to temp path");
                    entry.ExtractToFile(tempPath + "libil2cppx86", true);

                    if (Settings.Default.ExtractBinary)
                    {
                        WriteOutputInfo("Extract binary file to dump path");
                        entry.ExtractToFile(EnsureDirectoryForFile(outputPath + "\\x86\\libil2cpp.so"), true);
                    }

                    Dumper(tempPath + "libil2cppx86", tempPath + "global-metadata.dat", EnsureDirectoryForFile(outputPath + "\\x86\\"));
                }
            }
        });
    }

    private Task ApkSplitDumping(string file, string outputPath)
    {
        return Task.Factory.StartNew(() =>
        {
            using var archive = ZipFile.OpenRead(file);
            var binaryFile = archive.Entries.FirstOrDefault(f => f.Name.Contains("libil2cpp.so"));
            var metadataFile = archive.Entries.FirstOrDefault(f => f.FullName == "assets/bin/Data/Managed/Metadata/global-metadata.dat");

            if (metadataFile != null)
            {
                WriteOutputInfo("Extract global-metadata.dat to temp path");
                metadataFile.ExtractToFile(tempPath + "global-metadata.dat", true);

                if (Settings.Default.ExtractMetaData)
                {
                    WriteOutputInfo("Extract global-metadata.dat to dump path");
                    metadataFile.ExtractToFile(EnsureDirectoryForFile(outputPath + "\\global-metadata.dat"), true);
                }
            }

            if (binaryFile != null)
            {
                WriteOutputInfo("Extract binary file to temp path");
                binaryFile.ExtractToFile(tempPath + "libil2cpp.so", true);

                if (Settings.Default.ExtractBinary)
                {
                    WriteOutputInfo("Extract binary file to dump path");
                    binaryFile.ExtractToFile(EnsureDirectoryForFile(outputPath + "\\libil2cpp.so"), true);
                }
            }

            if (File.Exists(tempPath + "libil2cpp.so") && File.Exists(tempPath + "global-metadata.dat"))
            {
                Dumper(tempPath + "libil2cpp.so", tempPath + "global-metadata.dat", EnsureDirectoryForFile(outputPath + "\\"));
            }
        });
    }

    private Task ApksDumping(string file, string outputPath)
    {
        return Task.Factory.StartNew(() =>
        {
            using var archive = ZipFile.OpenRead(file);
            foreach (var entryApks in archive.Entries)
            {
                if (!entryApks.FullName.EndsWith(".apk", StringComparison.OrdinalIgnoreCase)) continue;

                var apkFile = Path.Combine(tempPath, entryApks.FullName);

                WriteOutputInfo($"Extract {entryApks.FullName} to temp path");
                entryApks.ExtractToFile(apkFile, true);

                using var entryBase = ZipFile.OpenRead(apkFile);
                var binaryFile = entryBase.Entries.FirstOrDefault(f => f.Name.Contains("libil2cpp.so"));
                var metadataFile = entryBase.Entries.FirstOrDefault(f => f.FullName == "assets/bin/Data/Managed/Metadata/global-metadata.dat");

                if (metadataFile != null)
                {
                    WriteOutputInfo("Extract global-metadata.dat to temp path");
                    metadataFile.ExtractToFile(tempPath + "global-metadata.dat", true);

                    if (Settings.Default.ExtractMetaData)
                    {
                        WriteOutputInfo("Extract global-metadata.dat to dump path");
                        metadataFile.ExtractToFile(EnsureDirectoryForFile(outputPath + "\\global-metadata.dat"), true);
                    }
                }

                if (binaryFile != null)
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.FullName.Equals("lib/armeabi-v7a/libil2cpp.so") && cbArch.SelectedIndex is 0 or 1)
                        {
                            WriteOutputInfo("Dumping ARMv7...");

                            WriteOutputInfo("Extract binary file to temp path");
                            entry.ExtractToFile(tempPath + "libil2cpparmv7", true);

                            if (Settings.Default.ExtractBinary)
                            {
                                WriteOutputInfo("Extract binary file to dump path");
                                entry.ExtractToFile(EnsureDirectoryForFile(outputPath + "\\ARMv7\\libil2cpp.so"), true);
                            }

                            Dumper(tempPath + "libil2cpparmv7", tempPath + "global-metadata.dat", EnsureDirectoryForFile(outputPath + "\\ARMv7\\"));
                        }

                        if (entry.FullName.Equals("lib/arm64-v8a/libil2cpp.so") && cbArch.SelectedIndex is 0 or 2)
                        {
                            WriteOutputInfo("Dumping ARM64...");

                            WriteOutputInfo("Extract binary file to temp path");
                            entry.ExtractToFile(tempPath + "libil2cpparm64", true);

                            if (Settings.Default.ExtractBinary)
                            {
                                WriteOutputInfo("Extract binary file to dump path");
                                entry.ExtractToFile(EnsureDirectoryForFile(outputPath + "\\ARM64\\libil2cpp.so"), true);
                            }

                            Dumper(tempPath + "libil2cpparm64", tempPath + "global-metadata.dat", EnsureDirectoryForFile(outputPath + "\\ARM64\\"));
                        }

                        if (entry.FullName.Equals("lib/x86/libil2cpp.so") && cbArch.SelectedIndex is 0)
                        {
                            WriteOutputInfo("Dumping x86...");

                            WriteOutputInfo("Extract binary file to temp path");
                            entry.ExtractToFile(tempPath + "libil2cppx86", true);

                            if (Settings.Default.ExtractBinary)
                            {
                                WriteOutputInfo("Extract binary file to dump path");
                                entry.ExtractToFile(EnsureDirectoryForFile(outputPath + "\\x86\\libil2cpp.so"), true);
                            }

                            Dumper(tempPath + "libil2cppx86", tempPath + "global-metadata.dat", EnsureDirectoryForFile(outputPath + "\\x86\\"));
                        }
                    }
                }

                entryBase.Dispose();
                File.Delete(apkFile);
            }
        });
    }

    #endregion Auto Dump

    #region Copy to clipboard

    private void menuCopy_Click(object sender, EventArgs e)
    {
        Clipboard.SetText(rbLog.SelectedText);
    }

    private void rbLog_TextChanged(object sender, EventArgs e)
    {
        rbLog.SelectionStart = rbLog.Text.Length;
        rbLog.ScrollToCaret();
    }

    #endregion Copy to clipboard

    #region Logging

    public static void AppendText(RichTextBox box, string text, Color color)
    {
        box.SelectionStart = box.TextLength;
        box.SelectionLength = 0;
        box.SelectionColor = color;
        box.AppendText(text);
        box.SelectionColor = box.ForeColor;
        box.ScrollToCaret();
    }

    private void TextToLogs(string str, Color color)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(() => AppendText(rbLog, str, color)));
            return;
        }

        AppendText(rbLog, str, color);
    }

    public void WriteOutputColor(string str, Color color)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(() => WriteOutputColor(str, color)));
            return;
        }

        TextToLogs(str + Environment.NewLine, color);
    }

    public void WriteOutputSucceed(string str)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(() => WriteOutputColor(str, Color.Green)));
            return;
        }

        TextToLogs(str + Environment.NewLine, Color.Green);
    }

    public void WriteOutputInfo(string str)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(() => WriteOutputColor(str, Color.Black)));
            return;
        }

        TextToLogs(str + Environment.NewLine, Color.Black);
    }

    public void WriteOutputWarning(string str)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(() => WriteOutputColor(str, Color.OrangeRed)));
            return;
        }

        TextToLogs(str + Environment.NewLine, Color.OrangeRed);
    }

    public void WriteOutputError(string str)
    {
        if (InvokeRequired)
        {
            Invoke(new MethodInvoker(() => WriteOutputColor(str, Color.Red)));
            return;
        }

        TextToLogs(str + Environment.NewLine, Color.Red);
    }

    #endregion Logging

    #region Form Controller

    private void FormState(AppStatus appStatus)
    {
        try
        {
            if (appStatus == AppStatus.Running)
            {
                btnDump.Text = @"Dumping...";
                EnableController(this, false);
            }
            else
            {
                btnDump.Text = @"Dump";
                EnableController(this, true);
            }
        }
        catch (Exception e)
        {
            WriteOutputError("Form State: " + e.Message);
        }
    }

    private static void EnableController(Form form, bool value)
    {
        foreach (var obj in form.Controls)
        {
            var control = (Control)obj;
            switch (control.GetType().Name)
            {
                case "Button":
                case "TextBox":
                case "RadioButton":
                case "RichTextBox":
                case "ComboBox":
                    control.Enabled = value;
                    break;

                case "GroupBox":
                case "Panel":
                case "TableLayoutPanel":
                case "TabPage":
                    EnableController(control, value);
                    break;
            }
        }
    }

    private static void EnableController(Control control, bool value)
    {
        foreach (var obj in control.Controls)
        {
            var control2 = (Control)obj;
            switch (control2.GetType().Name)
            {
                case "Button":
                case "TextBox":
                case "RadioButton":
                case "RichTextBox":
                case "ComboBox":
                    control2.Enabled = value;
                    break;

                case "GroupBox":
                case "Panel":
                case "TableLayoutPanel":
                case "TabPage":
                    EnableController(control, value);
                    break;
            }
        }
    }

    #endregion Form Controller

    #region Utility

    private static void SetAndUpdate(ref string? backingField, string? newValue, TextBox textBox)
    {
        if (backingField == newValue) return;

        backingField = newValue;
        textBox.Text = newValue;
    }

    private static string EnsureDirectoryForFile(string path)
    {
        var directoryName = Path.GetDirectoryName(path);
        if (directoryName == null) return path;

        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
        return path;
    }

    private static void DeleteExistsFile(string file)
    {
        if (File.Exists(file))
        {
            File.Delete(file);
        }
    }

    #endregion Utility
}