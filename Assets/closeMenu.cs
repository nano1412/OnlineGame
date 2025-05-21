using UnityEngine;

public class closeMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveMyParentOff()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
