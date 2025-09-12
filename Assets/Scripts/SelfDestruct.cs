// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-07-16
// Description: Destroys the attached GameObject when called, used for timed or triggered self-destruction.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
