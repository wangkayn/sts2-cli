// Stub for MegaCrit.Sts2.addons.mega_text types
using Godot;

namespace MegaCrit.Sts2.addons.mega_text;

public class MegaRichTextLabel : RichTextLabel
{
    public new class SignalName : RichTextLabel.SignalName { }
    public new class MethodName : Node.MethodName { }
    public new class PropertyName : Node.PropertyName { }
    public bool FitContent { get; set; }
}

public class MegaLabel : Label
{
    public new class SignalName : Label.SignalName { }
}
