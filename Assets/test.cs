using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StudentVueAPI;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		StudentVue studentVue = new StudentVue();
		studentVue.Login("","","sis.powayusd.com");

		foreach (var x in studentVue.GetGradeBook()) {
			Debug.Log(x.CourseTitle);
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
