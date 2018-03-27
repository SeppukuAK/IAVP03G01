using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Tablero
{
    public Tile[,] Matriz { get; set; }

    public Tablero()
    {
        Matriz = new Tile[GameManager.ALTO, GameManager.ANCHO];
        ColocaTierra();
    }

    //Método que crea la matriz de tiles (tierra por defecto)
    public void ColocaTierra()
    {
        //Se rellenan todas las casillas con tierra al inicio
        for (int y = 0; y < GameManager.ALTO; y++)
        {
            for (int x = 0; x < GameManager.ANCHO; x++)
            {
                Matriz[y, x] = new Tile(Terreno.TIERRA, new Pos(x, y));

            }
        }
    }

    //Se le llama desde GM cuando se ha hecho click en un tile en el estado COLOCAAGUJERO
    public void ColocaAgujero(Pos pos)
    {
        Matriz[pos.y, pos.x].terreno = Terreno.AGUJERO;
        ColocaBarro(pos.x, pos.y);

    }

    //Método que establece el tipo de terreno según sea barro normal o barro con sangre
    public void ColocaBarro(int x, int y)
    {
        if (y + 1 < GameManager.ALTO && Matriz[y + 1, x].terreno != Terreno.AGUJERO)
            DeterminaSangre(Matriz[y + 1, x]);

        if (y - 1 >= 0 && Matriz[y - 1, x].terreno != Terreno.AGUJERO)
            DeterminaSangre(Matriz[y - 1, x]);

        if (x + 1 < GameManager.ANCHO && Matriz[y, x + 1].terreno != Terreno.AGUJERO)
            DeterminaSangre(Matriz[y, x + 1]);

        if (x - 1 >= 0 && Matriz[y, x - 1].terreno != Terreno.AGUJERO)
            DeterminaSangre(Matriz[y, x - 1]);
    }

    //Método que comprueba si el tile es sangre o barro
    void DeterminaSangre(Tile tile)
    {
        if (tile.terreno == Terreno.SANGRE)
            tile.terreno = Terreno.SANGREBARRO;
        else
            tile.terreno = Terreno.BARRO;

    }

    //Se le llama desde GM cuando se ha hecho click en un tile en el estado COLOCACADAVER
    public void ColocaSangre(int x, int y)
    {
        if (y + 1 < GameManager.ALTO)
            Matriz[y + 1, x].terreno = Terreno.SANGRE;

        if (y - 1 >= 0 )
            Matriz[y - 1, x].terreno = Terreno.SANGRE;

        if (x + 1 < GameManager.ANCHO)
            Matriz[y, x + 1].terreno = Terreno.SANGRE;

        if (x - 1 >= 0)
            Matriz[y, x - 1].terreno = Terreno.SANGRE;

    }
}
