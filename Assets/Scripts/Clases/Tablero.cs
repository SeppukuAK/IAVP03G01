using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Tablero
{
    public Tile[,] matriz { get; set; }

    public Tablero()
    {
        matriz = new Tile[ALTO, ANCHO];

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
        for (int y = 0; y < GameManager.ANCHO; y++)
        {
            for (int x = 0; x < GameManager.ALTO; x++)
            {
                matriz[y, x] = new Tile(Terreno.TIERRA, new Pos(x, y));

            }
        }
    }


    public void ColocaAgujero()
    {
        Random rnd = new Random();
        int random = rnd.Next(0, 20);

        // UnityEngine.Debug.Log(random);

        for (int y = 0; y < GameManager.ANCHO; y++)
        {
            for (int x = 0; x < GameManager.ALTO; x++)
            {
                if (random >= 9)
                {// && no contieneCadaver && no contieneCasa)
                    Pos posAgujero = new Pos(x, y);

                    if (!(GameManager.instance.HayCasa(posAgujero, GameManager.instance.GetPosCasa())))
                    {
                        matriz[y, x] = new Tile(Terreno.AGUJERO, new Pos(x, y));
                        ColocaBarro(x, y);
                    }
                }
                random = rnd.Next(0, 10);
            }
        }
    }

    public void ColocaBarro(int x, int y)
    {
        if (y + 1 < GameManager.ANCHO && matriz[x, y + 1].terreno != Terreno.AGUJERO)
            matriz[y + 1, x] = new Tile(Terreno.BARRO, new Pos(x, y + 1));

        if (y - 1 >= 0 && matriz[x, y - 1).terreno != Terreno.AGUJERO)
            matriz[y - 1, x] = new Tile(Terreno.BARRO, new Pos(x, y - 1));

        if (x + 1 < GameManager.ALTO && matriz[x + 1, y].terreno != Terreno.AGUJERO)
            matriz[y, x + 1] = new Tile(Terreno.BARRO, new Pos(x + 1, y));

        if (x - 1 >= 0 && matriz[x - 1, y].terreno != Terreno.AGUJERO)
            matriz[y, x - 1] = new Tile(Terreno.BARRO, new Pos(x - 1, y));

    }

}
