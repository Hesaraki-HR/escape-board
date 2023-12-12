using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods
{
    public static Transform FindInChildrenRecursive(this Transform parent, string name)
    {
        Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID).Where(x => x.name == name).FirstOrDefault();
        Transform result = parent.Find(name);

        if (result != null)
            return result;

        foreach (Transform child in parent)
        {
            result = child.FindInChildrenRecursive(name);
            if (result != null)
                return result;
        }

        return null;
    }

    public static Vector3 LimitToNorthernHemisphere(this Vector3 vector)
    {
        // Normalize the vector to get its direction
        Vector3 direction = vector.normalized;

        // Calculate the angle between the direction vector and the positive Y-axis
        float angle = Vector3.SignedAngle(direction, Vector3.up, Vector3.forward);

        // If the angle is positive (in the Northern hemisphere), return the original vector
        if (angle >= 0f)
        {
            return vector;
        }
        else
        {
            // If the angle is negative (in the Southern hemisphere), reflect the vector in the XZ plane
            return Vector3.Reflect(vector, Vector3.forward);
        }
    }

    public static Vector3 GetDeviatedDirectionFromUp(this Vector3 upVector, float maxAngleDeviation)
    {
        // Calculate a random rotation axis
        Vector3 randomAxis = Random.onUnitSphere;

        // Rotate the up vector by a random angle around the random axis
        Quaternion randomRotation = Quaternion.Euler(randomAxis * Random.Range(-maxAngleDeviation, maxAngleDeviation));
        Vector3 deviatedDirection = randomRotation * upVector;

        return deviatedDirection;
    }
}
