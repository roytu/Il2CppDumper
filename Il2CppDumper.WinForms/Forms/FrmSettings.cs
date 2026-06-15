namespace Il2CppDumper.WinForms.Forms;

public partial class FrmSettings : Form
{
    public FrmSettings()
    {
        InitializeComponent();
    }

    private void FrmSettings_Load(object sender, EventArgs e)
    {
        chkAutoSetDir.Checked = Settings.Default.AutoSetDir;
        rad64.Checked = Settings.Default.IsMachO64;
        chkExtBin.Checked = Settings.Default.ExtractBinary;
        chkExtDat.Checked = Settings.Default.ExtractMetaData;

        if (Settings.Default.OutputScripts == null) return;
        var checkedList = Settings.Default.OutputScripts.Cast<string>().ToList();
        var scriptsItems = clbScripts.Items.Cast<string>().ToList();

        var index = 0;
        foreach (var item in scriptsItems)
        {
            if (checkedList.Contains(item))
                clbScripts.SetItemChecked(index, true);
            index++;
        }
    }

    private void Save()
    {
        Settings.Default.AutoSetDir = chkAutoSetDir.Checked;
        Settings.Default.IsMachO64 = rad64.Checked;
        Settings.Default.ExtractBinary = chkExtBin.Checked;
        Settings.Default.ExtractMetaData = chkExtDat.Checked;
        Settings.Default.OutputScripts = [.. clbScripts.CheckedItems.Cast<string>()];
        Settings.Default.Save();
    }

    private void btnApply_Click(object sender, EventArgs e)
    {
        Close();
        Save();
    }
}