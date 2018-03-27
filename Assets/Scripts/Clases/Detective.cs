using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detective
{
    public Pos pos { get; set; }

    public Detective()
    {
        pos = new Pos(0, 0);
    }

    public Detective(Pos pos)
    {
        this.pos = pos;

    }


}
