
using UnityEngine;

public class Torus<Elem>
{

    static int doMod(int n, int m)
    {
        int ans = n % m;
        ans += m * (ans < 0 ? 1 : 0);
        return ans;
    }
    
    public int padding { get; private set; }

    int root_i, root_j;
    Elem[][] all_nodes;
    public Torus(int padding_)
    {
        padding = Mathf.Max(1, padding_);
        // todo

        all_nodes = new Elem[2 * padding][];
        for (int i = 0; i < 2 * padding; i++)
        {
            all_nodes[i] = new Elem[2 * padding];
        }
        root_i = 0;
        root_j = 0;
    }

    public Elem get(int i, int j)
    {
        return all_nodes[doMod(i + root_i, 2 * padding)][doMod(j + root_j, 2 * padding)];
    }

    public void set(int i, int j, Elem e)
    {
        all_nodes[doMod(i + root_i, 2 * padding)][doMod(j + root_j, 2 * padding)] = e;
    }

    public void rollRight()
    {
        root_i++;
    }

    public void rollLeft()
    {
        root_i--;
    }

    public void rollForward()
    {
        root_j++;
    }

    public void rollBack()
    {
        root_j--;
    }
}