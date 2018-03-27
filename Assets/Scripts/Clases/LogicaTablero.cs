using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LogicaTablero {

    LogicaTile [,] _matriz;

	public LogicaTablero()
    {
		_matriz = new LogicaTile[5, 10];

        ColocaTierra();
        //Coloca casa
        //Coloca cadaver
        //Si la sangre coincide con el barro, necesito un tile Barro con sangre para ese caso
        //ColocaAgujero();
            
    }


    public void ColocaTierra()
    {

        //Se rellenan todas las casillas con tierra al inicio
        //i son filas
        for (int y = 0; y < GameManager.Ancho; y++)
        {
            for (int x = 0; x < GameManager.Alto; x++)
            {
                _matriz[y, x] = new LogicaTile(Terreno.tierra, new Pos(x, y));

            }
        }
    }


      public void ColocaAgujero()
    {
        Random rnd = new Random();
        int random = rnd.Next(0, 20);

       // UnityEngine.Debug.Log(random);

        for (int y = 0; y < GameManager.Ancho; y++)
        {
            for (int x = 0; x < GameManager.Alto; x++)
            {
                if (random >= 9)
                {// && no contieneCadaver && no contieneCasa)
                    Pos posAgujero = new Pos(x, y);

                    if (!(GameManager.instance.HayCasa(posAgujero, GameManager.instance.GetPosCasa())))
                    {
                        _matriz[y, x] = new LogicaTile(Terreno.agujero, new Pos(x, y));
                        ColocaBarro(x, y);
                    }
                }
                random = rnd.Next(0, 10);
            }
        }
    }

    public void ColocaBarro(int x, int y)
    {
        if( y + 1 < GameManager.Ancho && GetLogicaTile(x,y+1).GetTerreno() != Terreno.agujero)
            _matriz[y + 1, x] = new LogicaTile(Terreno.barro, new Pos(x, y + 1));

        if (y - 1 >= 0 &&  GetLogicaTile(x, y - 1).GetTerreno() != Terreno.agujero)
            _matriz[y - 1, x] = new LogicaTile(Terreno.barro, new Pos(x, y - 1));

        if (x + 1 < GameManager.Alto && GetLogicaTile(x+1, y).GetTerreno() != Terreno.agujero)
            _matriz[y, x + 1] = new LogicaTile(Terreno.barro, new Pos(x + 1, y));

        if (x - 1 >= 0 && GetLogicaTile(x-1,y).GetTerreno() != Terreno.agujero)
            _matriz[y, x - 1] = new LogicaTile(Terreno.barro, new Pos(x - 1, y));

    }


	public LogicaTile GetLogicaTile(int x, int y) { return _matriz[y,x]; }
	public LogicaTile GetLogicaTile(Pos pos) { return _matriz[pos.GetY(), pos.GetX()]; }
	public LogicaTile[,] GetMatriz() { return _matriz; }


}
