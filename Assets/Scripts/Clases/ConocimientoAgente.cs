using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConocimientoAgente
{

    public Percepcion[,] Matriz { get; set; }

    List<Pos> fronteraSegura;
    List<Pos> fronteraRiesgo;

    public ConocimientoAgente(Tile tileIni)
    {
        Matriz = new Percepcion[GameManager.ALTO, GameManager.ANCHO];

        for (int i = 0; i < GameManager.ALTO; i++)
        {
            for (int j = 0; j < GameManager.ANCHO; j++)
            {
                Matriz[i, j] = Percepcion.INEXPLORADO;
            }
        }

        Matriz[tileIni.pos.y, tileIni.pos.x] = Percepcion.EXPLORADO;

        fronteraSegura = new List<Pos>();
        fronteraRiesgo = new List<Pos>();

        ActualizaFrontera(tileIni);
    }

    public void ActualizaConocimiento(Tile tile)
    {
        Matriz[tile.pos.y, tile.pos.x] = Percepcion.EXPLORADO;

        ActualizaFrontera(tile);
    }

    void ActualizaFrontera(Tile tile)
    {
        //Actualizo a la izquierda
        if (tile.pos.x > 0 )
        {
            ActualizaPercepcion(tile.terreno, tile.pos.x -1, tile.pos.y);
        }

        //Actualizo a la derecha
        if (tile.pos.x < GameManager.ANCHO - 1)
        {
            ActualizaPercepcion(tile.terreno, tile.pos.x + 1, tile.pos.y);

        }
        //Actualizo arriba
        if (tile.pos.y > 0)
        {
            ActualizaPercepcion(tile.terreno, tile.pos.x, tile.pos.y - 1);

        }

        //Actualizo abajo
        if (tile.pos.y < GameManager.ALTO - 1)
        {
            ActualizaPercepcion(tile.terreno, tile.pos.x, tile.pos.y + 1);

        }
    }

    void ActualizaPercepcion(Terreno terreno, int x, int y)
    {
        //Si la casilla no ha sido explorada ni establecida como segura
        if (Matriz[y, x] != Percepcion.EXPLORADO && Matriz[y, x] != Percepcion.SEGURO)
        {
            if ((terreno == Terreno.BARRO || terreno == Terreno.SANGREBARRO))
            {
                Matriz[y, x] = Percepcion.RIESGO;
                fronteraRiesgo.Add(new Pos(x, y));

            }
            else
            {
                Matriz[y, x] = Percepcion.SEGURO;
                fronteraSegura.Add(new Pos(x, y));

            }
        }
    }

    public Pos NextBestPos(Pos actualPos)
    {
        Pos nearest;
        if (fronteraSegura.Count > 0)
        {
            nearest = fronteraSegura
            .OrderBy(t => t.ManhattanDistance(actualPos))
            .FirstOrDefault();
        }
        else
        {
            nearest = fronteraRiesgo
            .OrderBy(t => t.ManhattanDistance(actualPos))
            .FirstOrDefault();
        }

        return nearest;
    }

}
