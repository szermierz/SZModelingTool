using UnityEngine;

namespace SZ.ModelingTool
{
    public class CreateSphereEffect : EffectBase
    {
        [SerializeField]
        private int m_interations = 0;

        [SerializeField]
        private float m_radius = 1.0f;

        protected override void EffectImplementation()
        {
            const float c_half = 0.7071068f;
            var halfRadius = c_half * m_radius;

            EnsureCloneModel(true, true);

            CreateVertex(halfRadius, 0, halfRadius);
            CreateVertex(halfRadius, 0, -halfRadius);
            CreateVertex(-halfRadius, 0, -halfRadius);
            CreateVertex(-halfRadius, 0, halfRadius);
            CreateVertex(0, m_radius, 0);
            CreateVertex(0, -m_radius, 0);

            CreateFace(0, 1, 4);
            CreateFace(0, 5, 1);
            CreateFace(3, 0, 4);
            CreateFace(3, 5, 0);
            CreateFace(3, 4, 2);
            CreateFace(2, 5, 3);
            CreateFace(1, 2, 4);
            CreateFace(1, 5, 2);

            for (int i = 0; i < m_interations; ++i)
            {
                var faces = EffectsRoot.DestFacesRoot.GetComponentsInChildren<Face>();
                foreach (var face in faces)
                {
                    var v0 = face.Vertices[0];
                    var v1 = face.Vertices[1];
                    var v2 = face.Vertices[2];
                    var v3Pos = ((v0.Position + v1.Position) / 2.0f).normalized;
                    var v3 = EffectsRoot.transform.GetChild(CreateVertex(v3Pos.x, v3Pos.y, v3Pos.z)).GetComponent<Vertex>();
                    var v4Pos = ((v1.Position + v2.Position) / 2.0f).normalized;
                    var v4 = EffectsRoot.transform.GetChild(CreateVertex(v4Pos.x, v4Pos.y, v4Pos.z)).GetComponent<Vertex>();
                    var v5Pos = ((v2.Position + v0.Position) / 2.0f).normalized;
                    var v5 = EffectsRoot.transform.GetChild(CreateVertex(v5Pos.x, v5Pos.y, v5Pos.z)).GetComponent<Vertex>();

                    face.Vertices = new[] { v0, v3, v5 };
                    CreateFace(IndexOf(v3), IndexOf(v1), IndexOf(v4));
                    CreateFace(IndexOf(v3), IndexOf(v4), IndexOf(v5));
                    CreateFace(IndexOf(v5), IndexOf(v4), IndexOf(v2));

                    int IndexOf(Vertex v) => v.transform.GetSiblingIndex();
                }
            }

            int CreateVertex(float x, float y, float z)
            {
                var v = new GameObject("V", typeof(Vertex));
                v.transform.SetParent(EffectsRoot.DestVerticesRoot.transform);
                v.transform.position = new Vector3(x, y, z);
                return v.transform.GetSiblingIndex();
            }

            void CreateFace(int index0, int index1, int index2)
            {
                var f = new GameObject("F", typeof(Face));
                f.transform.SetParent(EffectsRoot.DestFacesRoot.transform);
                f.GetComponent<Face>().Vertices = new[]
                {
                    EffectsRoot.DestVerticesRoot.transform.GetChild(index0).GetComponent<Vertex>(),
                    EffectsRoot.DestVerticesRoot.transform.GetChild(index1).GetComponent<Vertex>(),
                    EffectsRoot.DestVerticesRoot.transform.GetChild(index2).GetComponent<Vertex>(),
                };
            }
        }
    }
}