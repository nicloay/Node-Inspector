using UnityEngine;

namespace NodeInspector.Demo.OneToMany{ 
    [ShowAsProperty(parentType:typeof(ComplexItem))]
    public class ItemPack : IParentHolder<ComplexItem> {

        [OneToMany(DirectionType.Input, "entity")]
        public SimpleItem Source ;
        public int Quantity;
        [HideInInspector]
        public ComplexItem Parent;


        #region IParentHolder implementation
        public void SetParent(ComplexItem parent)
        {
            Parent = parent;
        }
        #endregion
    }
}