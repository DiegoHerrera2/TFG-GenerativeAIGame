using UnityEngine;

namespace _Scripts.Systems.Items
{
    public interface IEntity
    {
        public string Name { get; }
        public SpriteRenderer SpriteRenderer { get; }
        public Transform Transform { get; }
    }
}