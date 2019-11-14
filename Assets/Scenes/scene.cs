using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scene : MonoBehaviour
{
    // Start is called before the first frame update
    float timer1;

    void start()
    {
        timer1 = 0;
    }

    void Update()
    {
        timer1 += Time.deltaTime;
        Debug.Log(timer1);
        if(timer1>10)
        {
            Debug.Log("errwf");
            SceneManager.LoadScene(1);
        }
    }
}
