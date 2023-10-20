using System;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.Meshes
{
    /// <summary>
    /// A mesh in the form of a wedge with no height.
    /// </summary>
    [Serializable]
    public class Plane
    {
        public Mesh Mesh { get; }
        private Vector3[] _verts;
        private int[] _triangles;
        private bool _dynamic;
        private Vector2[] _uvs;

        public Plane(Vector2 size, bool dynamic = false)
        {
            this._dynamic = dynamic;
            Mesh = new Mesh();
            _verts = new Vector3[4];
            _triangles = new int[6];
            _uvs = new Vector2[_verts.Length];
            CreatePlane(size);
        }

        public Plane Regenerate(Vector2 size)
        {
            CreatePlane(size);
            return this;
        }

        private void CreatePlane(Vector2 size)
        {
            if (!Mesh) return;
            var halfX = size.x / 2;
            var halfZ= size.y / 2;
            var center = new Vector3(0, 0, halfZ);

            _verts[0] = center + new Vector3(-halfX, 0, -halfZ);
            _verts[1] = center + new Vector3(halfX, 0, -halfZ);
            _verts[2] = center + new Vector3(-halfX, 0, halfZ);
            _verts[3] = center + new Vector3(halfX, 0, halfZ);

            _triangles[0] = 0;
            _triangles[3] = _triangles[2] = 1;
            _triangles[4] = _triangles[1] = 2;
            _triangles[5] = 3;

            for (int i = 0; i < _uvs.Length; i++)
            {
                _uvs[i] = new Vector2(_verts[i].x, _verts[i].z);
            }

            Mesh.vertices = _verts;
            Mesh.triangles = _triangles;
            Mesh.uv = _uvs;
            Mesh.RecalculateNormals();
            if(_dynamic)
                Mesh.MarkDynamic();
            Mesh.name = "Plane";
        }
    }
}