using UnityEngine;
using UnityEngine.UI;

public class BareScript : MonoBehaviour
{
    [SerializeField]
    Image image;

    float val;

    public float MaxVal;

    Transform cam;

    private void Start() {
        cam = GameObject.FindWithTag("MainCamera").transform;
    }

    private void Update() {
        if(cam != null)
            transform.LookAt(cam.position);
    }

    public float Val
    {
        get
        {
            return val;
        }
        set
        {
            val = value; 
            UpdateValue();
        }
    }

    public void InitValue(float pm)
    {
        MaxVal = val = pm;

        UpdateValue();        
    }

    void UpdateValue()
    {
        val = Mathf.Clamp(val, 0, MaxVal);
        image.fillAmount = val/MaxVal;
    }
}
