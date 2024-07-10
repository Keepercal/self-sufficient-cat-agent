using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeaderGroupAttribute : PropertyAttribute
{
    public string header;
    
    public HeaderGroupAttribute(string header)
    {
        this.header = header;
    }
}
