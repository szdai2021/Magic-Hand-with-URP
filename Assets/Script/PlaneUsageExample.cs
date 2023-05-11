using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

/**
 * This class is an example of how to setup a cutting Plane from a GameObject
 * and how to work with coordinate systems.
 * 
 * When a Place slices a Mesh, the Mesh is in local coordinates whilst the Plane
 * is in world coordinates. The first step is to bring the Plane into the coordinate system
 * of the mesh we want to slice. This script shows how to do that.
 */
public class PlaneUsageExample : MonoBehaviour {

	public Material crossMat;
	public bool recursiveSlice;

	public SlicedHull SliceObject(GameObject obj, Material crossSectionMaterial) {
        // slice the provided object using the transforms of this object
        return obj.Slice(transform.position, transform.up, crossSectionMaterial);
	}

	public GameObject[] sliceObject(GameObject source)
	{
		SlicedHull hull = SliceObject(source, crossMat);

		if (hull != null)
		{
			GameObject lowerNull = hull.CreateLowerHull(source, crossMat);
			GameObject upperNull = hull.CreateUpperHull(source, crossMat);

			return new GameObject[] { lowerNull, upperNull };
		}
		else
		{
			return null;
		}
	}

}
