using UnityEngine;

public class Game : MonoBehaviour
{
    Camera cam;
    float y_began, y_ended, x;
    
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if(Global.GameOver)
            return;
        if (Input.touchCount == 1)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                y_began = ray.origin.y;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                y_ended = ray.origin.y;
                if (y_ended > y_began + 1||y_ended+1<y_began) //slide 
                {
                    x = ray.origin.x;
                    if(x<Global.SelectedGroupObjectx)
                    {
                        if(y_ended > y_began + 1)
                            Global.RotateClockWise=true;
                        else
                            Global.RotateClockWise=false;
                    }
                    if(x>Global.SelectedGroupObjectx)
                    {
                        if(y_ended > y_began + 1)
                            Global.RotateClockWise=false;
                        else
                            Global.RotateClockWise=true;
                    }
                    Global.Slide=true;
                }
                else //short touch to select a cell
                {
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
                    if (hit.collider != null)
                    {
                        hit.collider.gameObject.GetComponent<Cell>().Pressed(ray.origin.x, ray.origin.y);
                    }
                }
            }
        }
    }
}