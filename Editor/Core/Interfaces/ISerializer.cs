
namespace SZ.ModelingTool
{
    public interface ISerializer
    {
        string Serialize(Model model);
        string DefaultExtension { get; }
    }
}