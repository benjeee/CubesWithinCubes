using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateScene : MonoBehaviour {

    [SerializeField]
    Transform prefab;

    [SerializeField]
    Material debugMaterial;

    [SerializeField]
    int startingTries;

    [SerializeField]
    Material[] transparentStandardMaterials;

    [SerializeField]
    Material[] transparentSoftMaterials;

    [SerializeField]
    Material outerMaterial;

    [SerializeField]
    int materialInd;

    [SerializeField]
    int numLayers;

    const string baseName = "CUBE";
    int cubeLayer = 0;

	void Start () {
        Transform seed = Instantiate(prefab, transform.position, transform.rotation);
        seed.GetComponent<Renderer>().material = outerMaterial;

        seed.rotation = generateRandomRotation();

        seed.Translate(new Vector3(30, 0, 0));

        GenerateLayer(seed, numLayers, startingTries);
    }

    Quaternion generateRandomRotation()
    {
        float randomXRot = Random.Range(0, 360);
        float randomYRot = Random.Range(0, 360);
        float randomZRot = Random.Range(0, 360);
        return Quaternion.Euler(new Vector3(randomXRot, randomYRot, randomZRot));
    }

    Vector3 generateScale(Vector3 parentScale, int tries, int maxTries)
    {
        float maxScale = Mathf.Clamp(.8f * (tries / maxTries), .4f, .8f);
        float minScale = Mathf.Clamp(.6f * (tries / maxTries), .001f, .6f);
        float xScale = Random.Range(minScale, maxScale);
        float yScale = Random.Range(minScale, maxScale);
        float zScale = Random.Range(minScale, maxScale);

        return new Vector3(
            parentScale.x * xScale,
            parentScale.y * yScale,
            parentScale.x * zScale
        );
    }

    void GenerateLayer(Transform parent, int layersRemaining, int numTries)
    {
        if (layersRemaining <= 0) return;


        Vector3[] prevCorners = Get4Corners(parent);

        int tries = numTries;
        List<Transform> cubesOnThisLayer = new List<Transform>();
        while (tries > 0)
        {
            Transform newCube;
            Vector3 newPos = getRandomPointInCube(parent);
            Quaternion newRot = generateRandomRotation();
            Vector3 newScale = generateScale(parent.lossyScale, tries, numTries);

            newCube = Instantiate(prefab, newPos, newRot);
            newCube.localScale = newScale;

            if (!cubeInsideCube(newCube, parent))
            {
                Debug.Log("Cube not inside parent");
                Destroy(newCube.gameObject);
            }
            else if (collidesWithCubesOnLayer(newCube, cubesOnThisLayer))
            {
                Destroy(newCube.gameObject);
                Debug.Log("Cube collides w/ layer");
            }
            else
            {
                cubesOnThisLayer.Add(newCube);
                newCube.GetComponent<Renderer>().material = pickRandomMaterial(getMaterialList(materialInd));
                GenerateLayer(newCube, layersRemaining - 1, (int)numTries/2);
            }

            tries--;
        }

        
    }

    Material[] getMaterialList(int materialInd)
    {
        if (materialInd == 0) return transparentStandardMaterials;
        else if (materialInd == 1) return transparentSoftMaterials;
        else return transparentStandardMaterials;
    }

    Material pickRandomMaterial(Material[] materialList)
    {
        return materialList[(int)(Random.value * materialList.Length)];
    }

    //Missing pass thru case
    bool collidesWithCubesOnLayer(Transform cube, List<Transform> cubeList)
    {
        foreach(Transform queryCube in cubeList)
        {
            if (!(cubeSeparateFromCube(cube, queryCube))) return true;
        }

        return false;
    }



    Vector3 getRandomPointInCube(Transform cube)
    {
        Vector3 scale = cube.localScale;
        Vector3 forwardBack = cube.forward * scale.z * Random.Range(-.5f, .5f);
        Vector3 leftRight = cube.right * scale.x * Random.Range(-.5f, .5f);
        Vector3 upDown = cube.up * scale.y * Random.Range(-.5f, .5f);

        return forwardBack + leftRight + upDown + cube.position;
    }

    bool cubeSeparateFromCube(Transform cube, Transform queryCube)
    {
        Vector3[] cubeCorners = GetAllCorners(cube);
        Vector3[] queryCubeCorners = GetAllCorners(queryCube);

        foreach (Vector3 corner in cubeCorners)
        {
            if ((dotProductBetween(corner, queryCubeCorners[0], queryCubeCorners[1])
                && dotProductBetween(corner, queryCubeCorners[0], queryCubeCorners[2])
                && dotProductBetween(corner, queryCubeCorners[0], queryCubeCorners[3])))
                return false;

        }

        foreach (Vector3 corner in queryCubeCorners)
        {
            if ((dotProductBetween(corner, cubeCorners[0], cubeCorners[1])
                && dotProductBetween(corner, cubeCorners[0], cubeCorners[2])
                && dotProductBetween(corner, cubeCorners[0], cubeCorners[3])))
                return false;

        }
        return true;
    }
        
    bool cubeInsideCube(Transform smallCube, Transform bigCube)
    {
        Vector3[] smallCubeCorners = GetAllCorners(smallCube);
        Vector3[] bigCubeCorners = Get4Corners(bigCube);

        foreach (Vector3 corner in smallCubeCorners)
        {
            if (!(dotProductBetween(corner, bigCubeCorners[0], bigCubeCorners[1])
                && dotProductBetween(corner, bigCubeCorners[0], bigCubeCorners[2])
                && dotProductBetween(corner, bigCubeCorners[0], bigCubeCorners[3])))
                return false;

        }
        return true;
    }

    bool dotProductBetween(Vector3 x, Vector3 P, Vector3 Q)
    {
        Vector3 u = P - Q;
        float uP = Vector3.Dot(u, P);
        float uQ = Vector3.Dot(u, Q);
        float uX = Vector3.Dot(u, x);
        float max = Mathf.Max(uP, uQ);
        float min = Mathf.Min(uP, uQ);
        return min < uX && uX < max;
    }

    Vector3[] Get4Corners(Transform cube)
    {
        Quaternion rotation = cube.rotation;
        Matrix4x4 m = Matrix4x4.TRS(cube.position, cube.rotation, new Vector3(1, 1, 1));

        Vector3[] cornerList = new Vector3[4];
        Vector3 scale = cube.lossyScale;
        cornerList[0] = m.MultiplyPoint3x4(new Vector3(scale.x / 2, scale.y / 2, scale.z / 2));
        cornerList[1] = m.MultiplyPoint3x4(new Vector3(scale.x / 2, scale.y / 2, -(scale.z / 2)));
        cornerList[2] = m.MultiplyPoint3x4(new Vector3(scale.x / 2, -(scale.y / 2), scale.z / 2));
        cornerList[3] = m.MultiplyPoint3x4(new Vector3(-scale.x / 2, scale.y / 2, scale.z / 2));

        return cornerList;
    }

    Vector3[] GetAllCorners(Transform cube)
    {
        Quaternion rotation = cube.rotation;
        Matrix4x4 m = Matrix4x4.TRS(cube.position, cube.rotation, new Vector3(1, 1, 1));

        Vector3[] cornerList = new Vector3[8];
        Vector3 scale = cube.lossyScale;
        cornerList[0] = m.MultiplyPoint3x4(new Vector3(scale.x / 2, scale.y / 2, scale.z / 2));
        cornerList[1] = m.MultiplyPoint3x4(new Vector3(scale.x / 2, scale.y / 2, -scale.z / 2));
        cornerList[2] = m.MultiplyPoint3x4(new Vector3(scale.x / 2, -scale.y / 2, scale.z / 2));
        cornerList[3] = m.MultiplyPoint3x4(new Vector3(-scale.x / 2, scale.y / 2, scale.z / 2));
        cornerList[4] = m.MultiplyPoint3x4(new Vector3(scale.x / 2, -scale.y / 2, -scale.z / 2));
        cornerList[5] = m.MultiplyPoint3x4(new Vector3(-scale.x / 2, scale.y / 2, -scale.z / 2));
        cornerList[6] = m.MultiplyPoint3x4(new Vector3(-scale.x / 2, -scale.y / 2, scale.z / 2));
        cornerList[7] = m.MultiplyPoint3x4(new Vector3(-scale.x / 2, -scale.y / 2, -scale.z / 2));

        return cornerList;
    }
}
