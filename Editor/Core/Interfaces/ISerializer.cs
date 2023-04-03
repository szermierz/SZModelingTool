
namespace SZ.ModelingTool
{
    public interface ISerializer
    {
        string Serialize(Model model, string previous);

        ISerializer ParentSerializer
        {
            get
            {
                if (this is not UnityEngine.MonoBehaviour self)
                    return null;
                if (self.transform is not { } transform)
                    return null;
                if (transform.parent is not { } parent)
                    return null;
                if (parent.GetComponentInParent<ISerializer>() is not { } parentSerializer)
                    return null;

                return parentSerializer;
            }
        }
    }
}