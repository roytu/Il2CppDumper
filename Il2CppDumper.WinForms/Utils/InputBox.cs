namespace Il2CppDumper.WinForms.Utils;

public static class InputBox
{
    public static DialogResult Show(string text, string caption, ref string value, string? okMessage = null, string? cancelMessage = null)
    {
        var form = new Form();
        var label = new Label();
        var textBox = new TextBox();
        var buttonOk = new Button();
        var buttonCancel = new Button();


        label.AutoSize = true;
        label.SetBounds(9, 20, 372, 13);
        label.Text = text;

        textBox.Anchor |= AnchorStyles.Right;
        textBox.SetBounds(12, 36, 372, 20);
        textBox.Text = value;

        buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonOk.DialogResult = DialogResult.OK;
        buttonOk.SetBounds(228, 72, 75, 23);
        buttonOk.Text = okMessage ?? "OK";

        buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonCancel.DialogResult = DialogResult.Cancel;
        buttonCancel.SetBounds(309, 72, 75, 23);
        buttonCancel.Text = cancelMessage ?? "Cancel";

        form.AcceptButton = buttonOk;
        form.CancelButton = buttonCancel;
        form.Controls.AddRange(label, textBox, buttonOk, buttonCancel);
        form.ClientSize = new Size(Math.Max(300, label.Right + 10), 107);
        form.FormBorderStyle = FormBorderStyle.FixedDialog;
        form.MinimizeBox = false;
        form.MaximizeBox = false;
        form.StartPosition = FormStartPosition.CenterScreen;
        form.Text = caption;
        form.TopMost = true;

        var dialogResult = form.ShowDialog();
        value = textBox.Text;
        return dialogResult;
    }
}