using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Extensions
{
    public static class GameObjectExtensions
    {
        public static void DestroyAllChildren(this GameObject go, bool immediate)
        {
            int children = go.transform.childCount;
            for (int i = children-1; 0 <= i; i--)
            {
                if(immediate)
                    GameObject.DestroyImmediate(go.transform.GetChild(i).gameObject);
                else
                    GameObject.Destroy(go.transform.GetChild(i).gameObject);
            }
        }

        public static bool IsInLayermask(this GameObject go, LayerMask mask) => (mask & (1 << go.layer)) != 0;
    }
}
