using UnityEngine;

public class SetFrameRate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        {
            Application.targetFrameRate = 60;
        }
    }
}
