using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRPortalToolkit
{
    public class onColliderRenderchange : MonoBehaviour
    {

        public Material outline;


        private void OnCollisionEnter(Collision collision)
        {
            GameObject target = collision.collider.gameObject;

            // add the outline effect as the second render material
            Material[] materials = target.GetComponent<MeshRenderer>().materials;

            if (materials.Length == 1)
            {
                Array.Resize(ref materials, materials.Length + 1);
                materials[materials.Length - 1] = outline;
                target.GetComponent<MeshRenderer>().materials = materials;
            }

            print(collision.collider.name);
        }

        private void OnCollisionExit(Collision collision)
        {
            GameObject target = collision.collider.gameObject;

            // add the outline effect as the second render material
            Material[] materials = target.GetComponent<MeshRenderer>().materials;

            // remove the outline effect by deleting the second render material
            if (target.GetComponent<MeshRenderer>().materials.Length >= 2)
            {
                Array.Resize(ref materials, 1);
                target.GetComponent<MeshRenderer>().materials = materials;
            }
        }

    }
}
