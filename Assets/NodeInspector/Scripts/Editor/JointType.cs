using UnityEngine;
using System.Collections;

namespace NodeInspector.Editor{
    public enum JointType {
        Nan,
        OneToOne_IN,
        OneToMany_IN,
        ManyToOne_IN,
        OneToOne_OUT,
        OneToMany_OUT,
        ManyToOne_Out,
        ManyToMany_IN,
        ManyToMany_OUT,
        OneWay_OUT,
        OneWay_IN //This works only for class attribute
    }
}
