using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    //--------ATRIBUTOS--------------
    //Posibles Scripts
    public Pos PosCasa { get; set; }

    public Estado Estado { get; set; }
    public GameObject[,] MatrizTiles { get; set; }//matriz de GO tiles

    //Macros
    public const int ANCHO = 10;
    public const int ALTO = 5;
    public const int NUMAGUJEROS = 3;
    public const float DISTANCIA = 0.64f;

    Tablero tablero;
   
    int numAgujeros;//numero de agujeros que puede colocar el usuario

    private Detective agente;
    private GameObject agenteGO;
    private GameObject armaGO;
    private GameObject cadaverGO;

    //--------ATRIBUTOS--------------

    //--------ATRIBUTOS UNITY--------

    public GameObject tilePrefab;
    public GameObject casaPrefab;
    public GameObject cadaverPrefab;
    public GameObject detectivePrefab;
    public GameObject armaPrefab;

    public Sprite spriteBarro;
    public Sprite spriteAgujero;
    public Sprite spriteSangre;
    public Sprite spriteSangreBarro;

    public Button Button;

    //--------ATRIBUTOS UNITY--------


    //----------INICIALIZACION--------------
    void Start()
    {
        instance = this;

        Estado = Estado.COLOCACADAVER;
        numAgujeros = NUMAGUJEROS;

        MatrizTiles = new GameObject[ALTO, ANCHO];
        tablero = new Tablero();

        ColocaTablero();

    }

    //Pasa la representación lógica del tablero (matriz) a la representación física (gameobjects)
    void ColocaTablero()
    {
        GameObject GOTablero = new GameObject("Tablero");

        for (int y = 0; y < GameManager.ALTO; y++)
        {
            for (int x = 0; x < GameManager.ANCHO; x++)
            {
                //Creamos gameObject
                MatrizTiles[y, x] = Instantiate(tilePrefab, new Vector3(x * DISTANCIA, -y * DISTANCIA, 0), Quaternion.identity, GOTablero.transform);

                Tile tileAux = tablero.Matriz[y, x];

                //Casilla
                MatrizTiles[y, x].GetComponent<TileView>().ConstruyeCasilla(tileAux);
            }

        }

        PosCasa = new Pos(Random.Range(0, ANCHO), Random.Range(0, ALTO));
        GameObject casa = Instantiate(casaPrefab, new Vector3(PosCasa.x * DISTANCIA, -PosCasa.y * DISTANCIA, 0), Quaternion.identity);
    }

    //----------INICIALIZACION--------------



    //-------Se le llama al colocar el cadaver-------------------

    //Se le llama cuando el usuario hace click desde TileView
    public void ColocaCadaver(Pos pos)
    {
        tablero.ColocaCadaver(pos.x, pos.y);

        cadaverGO = Instantiate(cadaverPrefab, new Vector3(pos.x * DISTANCIA, -pos.y * DISTANCIA, 0), Quaternion.identity);


        //Cambiamos el estado
        Estado = Estado.COLOCAAGUJERO;
    }

    //------Se le llama desde Tablero.ColocaCadaver()------------

    public void ColocaSpriteSangre(int x, int y)
    {
        MatrizTiles[y, x].GetComponent<SpriteRenderer>().sprite = spriteSangre;
    }

    public void ColocaSpriteArma(int x, int y)
    {
        armaGO = Instantiate(armaPrefab, new Vector3(x * DISTANCIA, -y * DISTANCIA, 0), Quaternion.identity);
    }

    //------Se le llama desde Tablero.ColocaCadaver()----------
    //-------Se le llama al colocar el cadaver-------------------//


    //-------Se le llama al colocar el agujero-------------------//

    //Se le llama cuando el usuario hace click desde TileView
    public void ColocaAgujero(Pos pos)
    {
        numAgujeros--; //Se reduce el numero de agujeros a colocar
        tablero.ColocaAgujero(pos); //coloca el agujero en la matriz logica

        //Aplicamos el sprite
        MatrizTiles[pos.y, pos.x].GetComponent<SpriteRenderer>().sprite = spriteAgujero;

        //Cuando el numero de agujeros es 0 pasamos al estado de Pausa, antes de que la IA empiece
        if (numAgujeros == 0)
        {
            Estado = Estado.PAUSA;
            Button.gameObject.SetActive(true);

            agenteGO = Instantiate(detectivePrefab, new Vector3(PosCasa.x * DISTANCIA, -PosCasa.y * DISTANCIA, 0), Quaternion.identity);

            agente = new Detective(tablero.Matriz[PosCasa.y, PosCasa.x]);

            for (int y = 0; y < GameManager.ALTO; y++)
            {
                for (int x = 0; x < GameManager.ANCHO; x++)
                {
                    //Casilla
                    MatrizTiles[y, x].GetComponent<SpriteRenderer>().color = Color.black;
                }

            }
            MatrizTiles[PosCasa.y, PosCasa.x].GetComponent<SpriteRenderer>().color = Color.white;

            armaGO.GetComponent<SpriteRenderer>().color = Color.black;
            cadaverGO.GetComponent<SpriteRenderer>().color = Color.black;
        }
    }

    public void ColocaSpriteBarro(int x, int y) 
    {
        if (tablero.Matriz[y, x].Sangre)
            MatrizTiles[y, x].GetComponent<SpriteRenderer>().sprite = spriteSangreBarro;

        else
            MatrizTiles[y, x].GetComponent<SpriteRenderer>().sprite = spriteBarro;

    }

    //-------Se le llama al colocar el agujero-------------------//

    //Métodos de comprobacion

    public bool HayCasa(Pos pos)
    {
        return (pos.Equals(PosCasa));
    }


    //Se le llama al pulsar el boton
    public void AvanzaBusqueda()
    {

        agente.AvanzaAPos();

   
    }

    //Necesario para conocimientoAgente
    public Tile GetTile(Pos pos)
    {
        return tablero.Matriz[pos.y, pos.x];
    }

 

    public void MoverAgente(Stack<Pos> camino)
    {
        if (camino != null)
        StartCoroutine("AvanzaUnPaso", camino);

    }

    IEnumerator AvanzaUnPaso(Stack<Pos> camino)
    {
        Button.gameObject.SetActive(false);
        while (camino.Count > 0)
        {
            Pos pos = camino.Pop();

            agente.pos = pos;
            agenteGO.transform.position = new Vector3(pos.x * DISTANCIA, -pos.y * DISTANCIA, 0);

            yield return new WaitForSeconds(0.2f);

        }
        Button.gameObject.SetActive(true);

    }


    public void Renderiza (Pos pos)
    {
        MatrizTiles[pos.y, pos.x].GetComponent<SpriteRenderer>().color = Color.white;
    }
}
