using System.Collections.Generic;
using UnityEngine;

namespace SZ.ModelingTool
{
    public class CreatePlaneEffect : EffectBase
    {
        [SerializeField]
        private Vector2Int m_fields;

        [SerializeField]
        private Vector2Int m_fieldsOffset;

        [SerializeField]
        private float m_fieldSize;

        protected override void EffectImplementation()
        {
            // Create vertices
            var vertices = new List<List<Vertex>>(m_fields.x);
            for (int x = 0; x < m_fields.x; ++x)
            {
                var column = new List<Vertex>(m_fields.y);
                vertices.Add(column);
                for (int y = 0; y < m_fields.y; ++y)
                {
                    var vertexGO = new GameObject(nameof(Vertex), typeof(Vertex));
                    vertexGO.transform.SetParent(EffectsRoot.DestVerticesRoot.transform);

                    var vertex = vertexGO.GetComponent<Vertex>();
                    column.Add(vertex);

                    var xx = x + m_fieldsOffset.x;
                    var yy = y + m_fieldsOffset.y;

                    vertexGO.transform.position = new Vector3(xx * m_fieldSize, 0.0f, yy * m_fieldSize);
                }
            }

            // Create faces
            for (int x = 0; x < m_fields.x - 1; ++x)
            {
                for (int y = 0; y < m_fields.y - 1; ++y)
                {
                    var v0 = vertices[x + 0][y + 0];
                    var v1 = vertices[x + 1][y + 0];
                    var v2 = vertices[x + 0][y + 1];
                    var v3 = vertices[x + 1][y + 1];

                    var face1GO = new GameObject(nameof(Face), typeof(Face));
                    face1GO.transform.SetParent(EffectsRoot.DestFacesRoot.transform);
                    var face1 = face1GO.GetComponent<Face>();

                    face1.Vertices = new[] {v0, v3, v1};

                    var face2GO = new GameObject(nameof(Face), typeof(Face));
                    face2GO.transform.SetParent(EffectsRoot.DestFacesRoot.transform);
                    var face2 = face2GO.GetComponent<Face>();

                    face2.Vertices = new[] {v0, v2, v3};
                }
            }
        }
    }
}