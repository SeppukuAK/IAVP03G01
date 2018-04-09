using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConocimientoAgente
{
    EstadoAgente Estado { get; set; }

    public TilePercepcion[,] Matriz { get; set; }

    List<Pos> fronteraPrio;
    List<Pos> fronteraSegura;
    List<Pos> fronteraRiesgo;

    public ConocimientoAgente(Tile tileIni)
    {
        Matriz = new TilePercepcion[GameManager.ALTO, GameManager.ANCHO];

        for (int i = 0; i < GameManager.ALTO; i++)
        {
            for (int j = 0; j < GameManager.ANCHO; j++)
            {
                Matriz[i, j] = new TilePercepcion();
            }
        }

        fronteraSegura = new List<Pos>();
        fronteraRiesgo = new List<Pos>();
        fronteraPrio = new List<Pos>();


        ActualizaConocimiento(tileIni);

        //COMPROBAR SI PASO A BUSCAARMA,BUSCACUERPO O BUSCA CRIMEN
        if (fronteraPrio.Count > 0)
            Estado = EstadoAgente.BUSCAARMA;

        else if (fronteraSegura.Count > 0)
            Estado = EstadoAgente.EXPLORASEGURO;
        else if (fronteraRiesgo.Count > 0)
            Estado = EstadoAgente.EXPLORARIESGO;

    }

    public void ActualizaConocimiento(Tile tile)
    {
        Matriz[tile.pos.y, tile.pos.x].Percepcion = TipoPercepcion.EXPLORADO;

        ActualizaFrontera(tile);
    }

    //Llama a ActualizarPercepcion de todas las casillas adyacentes
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

    //Actualiza la percepción de una casilla a segura, insegura o prioritaria
    void ActualizaPercepcion(Terreno terreno, int x, int y)
    {
        //Si la casilla no ha sido explorada
        if (Matriz[y, x].Percepcion != TipoPercepcion.EXPLORADO)
        {
            //Si la casilla no la he marcado ya como prioritaria
            if (Matriz[y, x].Percepcion != TipoPercepcion.PRIORITARIO)
            {
                if (terreno == Terreno.SANGRE)
                {
                    Matriz[y, x].Percepcion = TipoPercepcion.PRIORITARIO;
                    fronteraPrio.Add(new Pos(x, y));

                }

                //Si es segura, ya no me interesa saber más
                else if (Matriz[y, x].Percepcion != TipoPercepcion.SEGURO)
                {
                    if (terreno == Terreno.TIERRA)
                    {
                        Matriz[y, x].Percepcion = TipoPercepcion.SEGURO;
                        fronteraRiesgo.Add(new Pos(x, y));

                    }

                    else if (Matriz[y, x].Percepcion != TipoPercepcion.RIESGO)
                    { 
                        if (terreno == Terreno.BARRO)
                        {
                            Matriz[y, x].Percepcion = TipoPercepcion.RIESGO;
                            fronteraSegura.Add(new Pos(x, y));

                        }
                    }
                }
            }



        }

    }

    //Devuelve la siguiente mejor posición, teniendo en cuenta el estado en el que se encuentra
    public Pos NextBestPos(Pos actualPos)
    {
        Pos nearest;
        if (fronteraSegura.Count > 0)
        {
            nearest = fronteraSegura
            .OrderBy(t => t.ManhattanDistance(actualPos))
            .FirstOrDefault();
            fronteraSegura.Remove(nearest);

        }
        else
        {
            nearest = fronteraRiesgo
            .OrderBy(t => t.ManhattanDistance(actualPos))
            .FirstOrDefault();
            fronteraRiesgo.Remove(nearest);
        }


        return nearest;
    }

}
