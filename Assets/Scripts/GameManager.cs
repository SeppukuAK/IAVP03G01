using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public const float DISTANCIA = 0.64f;

    Tablero tablero;
   
    public bool Pausa { get; set; }

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

    public Button ButtonComienzaBusqueda;
    public Button ButtonPausaBusqueda;
    public Button ButtonReiniciaBusqueda;

    public Image IconoArma;

    //--------ATRIBUTOS UNITY--------
    private void Awake()
    {
        instance = this;

    }

    //----------INICIALIZACION--------------
    void Start()
    {
        Pausa = false;
        IconoArma.enabled = false;

        ButtonComienzaBusqueda.gameObject.SetActive(false);
        ButtonPausaBusqueda.gameObject.SetActive(false);
        ButtonReiniciaBusqueda.gameObject.SetActive(false);


        Estado = Estado.COLOCACADAVER;

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

        ButtonComienzaBusqueda.gameObject.SetActive(true);
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
        tablero.ColocaAgujero(pos); //coloca el agujero en la matriz logica

        //Aplicamos el sprite
        MatrizTiles[pos.y, pos.x].GetComponent<SpriteRenderer>().sprite = spriteAgujero;
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


    //-------CALLBACKS DE BOTONES-------------------//

    //Boton ComienzaBosqueda
    public void IniciaBusqueda()
    {
        ButtonComienzaBusqueda.gameObject.SetActive(false);
        ButtonPausaBusqueda.gameObject.SetActive(true);
        ButtonReiniciaBusqueda.gameObject.SetActive(true);

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

        armaGO.GetComponent<SpriteRenderer>().enabled = false;
        cadaverGO.GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine("ContinuaBusqueda");
    }

    //Boton Pausa
    public void PausaBusqueda()
    {
        Pausa = !Pausa;
        if (!Pausa)
            StartCoroutine("ContinuaBusqueda");

    }
    //Boton Reinicio
    public void ReiniciaBusqueda()
    {
        SceneManager.LoadScene("Practica3");

    }
    //-------CALLBACKS DE BOTONES-------------------//

    private bool haciendoCamino = false;
    IEnumerator ContinuaBusqueda()
    {
        while (!agente.ObjetivoCumplido() && !Pausa )
        {
            if (!haciendoCamino)
            {
                haciendoCamino = true;
                agente.AvanzaAPos();
            }

             yield return new WaitForSeconds(0.016f);
        }

        while(haciendoCamino)
            yield return new WaitForSeconds(0.016f);

        if (!Pausa)
            agente.VuelveACasa();

        yield return null;

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

        while (camino.Count > 0)
        {
            Pos pos = camino.Pop();

            agente.pos = pos;
            agenteGO.transform.position = new Vector3(pos.x * DISTANCIA, -pos.y * DISTANCIA, 0);

            yield return new WaitForSeconds(0.2f);
        }

        haciendoCamino = false;
        yield return null;
    }


    public void Renderiza (Pos pos)
    {
        MatrizTiles[pos.y, pos.x].GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void RenderizaCadaver()
    {
        cadaverGO.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void RenderizaArma()
    {
        IconoArma.enabled = true;

    }
}
