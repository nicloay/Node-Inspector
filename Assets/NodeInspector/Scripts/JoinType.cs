using UnityEngine;
using System.Collections;

namespace NodeInspector{
    public enum JoinType {
        Nan,
        OneToOne_IN,
        OneToMany_IN,
        ManyToOne_IN,
        OneToOne_OUT,
        OneToMany_OUT,
        ManyToOne_Out,
        ManyToMany_IN,
        ManyToMany_OUT,
        OneToOne_Incognito_OUT,
        OneToMany_Incognito_OUT,
        Incognito_In //This works only for class attribute
    }
}
