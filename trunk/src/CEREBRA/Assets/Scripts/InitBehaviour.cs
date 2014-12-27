using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

public class InitBehaviour : MonoBehaviour
{
    SimpleUI.UI CEREBRAUI;

    SimpleUI.UIList processorListElem;
    string lastLoadedData;
    List<string[]> processorConfigs = new List<string[]>();
    List<libsimple.IProcessor> processors = new List<libsimple.IProcessor>();
    SimpleUI.UISlider layerSlider;
    SimpleUI.UISlider layerSlidery;
    SimpleUI.UISlider layerSliderz;
    SimpleUI.UISlider TransparencySlider;
    SimpleUI.UIButton exitPP; //exit potato print operation without selecting any slice
    float exit = 0; //variable for exitPP button
    libsimple.Packet lastLoadedPacket;
    libsimple.Packet packetToDisplay;
    libsimple.Packet potatoPacket;
    int percent;
    float layerValueX;
    float layerValueY;
    float layerValueZ;
    float transparency;
    string sliceAxis = "Y";
    int whichQuadrat = 0;
    int jumpCount = 4;
    int index = -1;
    float maxVal;
    SimpleUI.UIButton forwardButton;
    SimpleUI.UIButton backButton;
    void loadFile(string filename)
    {
        try
        {
            // load given file and get types(classes) in the file
            Assembly asm = Assembly.LoadFile(filename);
            System.Type[] tyyp = asm.GetTypes();
            foreach (System.Type typ in tyyp)
            {
                UnityEngine.Debug.Log(typ.FullName);
                System.Type[] ints = typ.GetInterfaces();
                // check if it implements IProcessor
                foreach (System.Type intType in ints)
                {
                    if (intType == typeof(libsimple.IProcessor))
                    {
                        try
                        {
                            // register
                            libsimple.IProcessor ip = (libsimple.IProcessor)asm.CreateInstance(typ.FullName);
                            libsimple.ProcessorManager.Register(ip);
                        }
                        catch (System.Exception)
                        {
                            ;
                        }
                        break;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e.ToString());
        }
    }

    void loadDirectory(string directory)
    {
        string currDir = System.IO.Directory.GetCurrentDirectory();
        System.IO.Directory.SetCurrentDirectory(directory);
        foreach (string name in System.IO.Directory.GetFiles("."))
        {
            UnityEngine.Debug.Log(name);
            if (name.ToLower().EndsWith(".dll"))
            {
                loadFile(name);
            }
        }
        System.IO.Directory.SetCurrentDirectory(currDir);
    }
    SimpleUI.UILabel labelQuad;
    // Use this for initialization
    void Start()
    {
        percent = 1;
        loadDirectory("processors");
        CSVLoader c = new CSVLoader();
        libsimple.ProcessorManager.Register(c);

        UnityEngine.Debug.Log("Listing registered Processors...");
        foreach (string s in libsimple.ProcessorManager.GetRegisteredProcessors())
        {
            UnityEngine.Debug.Log(s);
        }
        UnityEngine.Debug.Log("Listing done.");

        CEREBRAUI = new SimpleUI.UI();

        SimpleUI.UIPanel configPanel = new SimpleUI.UIPanel(new Vector2(0.8f, 0.0f), new Vector2(0.2f, 1.0f));
        SimpleUI.UIButton loadButton = new SimpleUI.UIButton("Load Data");
        SimpleUI.UIButton reloadButton = new SimpleUI.UIButton("Reload");
        SimpleUI.UIButton saveButton = new SimpleUI.UIButton("Save");
        SimpleUI.UIButton mniButton = new SimpleUI.UIButton("MNI Conversion");
        forwardButton = new SimpleUI.UIButton("Forward");
        backButton = new SimpleUI.UIButton("Back");


        processorListElem = new SimpleUI.UIList(new Vector2(0.01f, 0.05f), new Vector2(0.18f, 0.4f));
        processorListElem.Data = new System.Collections.Generic.List<string>();
        processorListElem.onDoubleClick += processorList_onDoubleClick;
        configPanel.Add(processorListElem);

        float scaling = 0.03f / Screen.height * Screen.width;

        SimpleUI.UILabel processorListLabel = new SimpleUI.UILabel("List of Applied Processors");
        processorListLabel.Size = new Vector2(0.2f, scaling);
        processorListLabel.Position = new Vector2(0.85f, 0.01f);

        SimpleUI.UIButton addButton = new SimpleUI.UIButton("+");
        addButton.Size = new Vector2(0.04f, scaling);
        addButton.Position = new Vector2(0.008f, 0.45f);
        addButton.onClick += addButton_onClick;

        SimpleUI.UIButton delButton = new SimpleUI.UIButton("-");
        delButton.Size = new Vector2(0.04f, scaling);
        delButton.Position = new Vector2(0.056f, 0.45f);
        delButton.onClick += delButton_onClick;

        SimpleUI.UIButton upButton = new SimpleUI.UIButton("^");
        upButton.Size = new Vector2(0.04f, scaling);
        upButton.Position = new Vector2(0.104f, 0.45f);
        upButton.onClick += upButton_onClick;

        SimpleUI.UIButton downButton = new SimpleUI.UIButton("v");
        downButton.Size = new Vector2(0.04f, scaling);
        downButton.Position = new Vector2(0.152f, 0.45f);
        downButton.onClick += downButton_onClick;

        SimpleUI.UIHGroup hg = new SimpleUI.UIHGroup();
        hg.Add(addButton);
        hg.Add(delButton);
        hg.Add(upButton);
        hg.Add(downButton);

        configPanel.Add(hg);

        SimpleUI.UILabel sliderLabel = new SimpleUI.UILabel("Layer Depth in X Direction");
        sliderLabel.Size = new Vector2(0.15f, 0.05f);
        sliderLabel.Position = new Vector2(0.81f, 0.52f);

        layerSlider = new SimpleUI.UISlider();
        layerSlider.Size = new Vector2(0.1f, 0.05f);
        layerSlider.Position = new Vector2(0.81f, 0.55f);
        layerSlider.onChange += layerSlider_onChange;

        // new sliders
        SimpleUI.UILabel sliderLabely = new SimpleUI.UILabel("Layer Depth in Y Direction");
        sliderLabely.Size = new Vector2(0.15f, 0.05f);
        sliderLabely.Position = new Vector2(0.81f, 0.58f);

        layerSlidery = new SimpleUI.UISlider();
        layerSlidery.Size = new Vector2(0.1f, 0.05f);
        layerSlidery.Position = new Vector2(0.81f, 0.61f);
        layerSlidery.onChange += layerSlidery_onChange;

        SimpleUI.UILabel sliderLabelz = new SimpleUI.UILabel("Layer Depth in Z Direction");
        sliderLabelz.Size = new Vector2(0.15f, 0.05f);
        sliderLabelz.Position = new Vector2(0.81f, 0.64f);

        layerSliderz = new SimpleUI.UISlider();
        layerSliderz.Size = new Vector2(0.1f, 0.05f);
        layerSliderz.Position = new Vector2(0.81f, 0.66f);
        layerSliderz.onChange += layerSliderz_onChange;

        SimpleUI.UILabel sliderTransparency = new SimpleUI.UILabel("Set Tranparency");
        sliderTransparency.Size = new Vector2(0.15f, 0.05f);
        sliderTransparency.Position = new Vector2(0.81f, 0.69f);

        TransparencySlider = new SimpleUI.UISlider();
        TransparencySlider.Size = new Vector2(0.1f, 0.05f);
        TransparencySlider.Position = new Vector2(0.81f, 0.72f);
        TransparencySlider.maxValue = 100; // transparency 0-1 arasinda
        TransparencySlider.hSliderValue = 50;
        TransparencySlider.onChange += TransparencySlider_onChange;

        loadButton.Size = new Vector2(0.05f, 0.025f);
        loadButton.Position = new Vector2(0.815f, 0.95f);
        loadButton.onClick += loadButton_onClick;

        reloadButton.Size = new Vector2(0.05f, 0.025f);
        reloadButton.Position = new Vector2(0.875f, 0.95f);
        reloadButton.onClick += reloadButton_onClick;


        mniButton.Size = new Vector2(0.085f, 0.025f);
        mniButton.Position = new Vector2(0.9f, 0.85f);
        mniButton.onClick += mniButton_onClick;

        labelQuad = new SimpleUI.UILabel((whichQuadrat * jumpCount + 1) + "-" + (whichQuadrat * jumpCount + jumpCount));
        labelQuad.Size = new Vector2(0.2f, scaling);
        labelQuad.Position = new Vector2(0.95f, 0.75f);

        forwardButton.Size = new Vector2(0.05f, 0.025f);
        forwardButton.Position = new Vector2(0.875f, 0.80f);
        forwardButton.onClick += forwardButton_onClick;

        backButton.Size = new Vector2(0.05f, 0.025f);
        backButton.Position = new Vector2(0.815f, 0.80f);
        backButton.onClick += backButton_onClick;

        saveButton.Size = new Vector2(0.05f, 0.025f);
        saveButton.Position = new Vector2(0.815f, 0.85f);
        saveButton.onClick += saveButton_onClick;

        SimpleUI.UIButton exitButton = new SimpleUI.UIButton("Exit");
        exitButton.Size = new Vector2(0.05f, 0.025f);
        exitButton.Position = new Vector2(0.935f, 0.95f);
        exitButton.onClick += exitButton_onClick;




        CEREBRAUI.Add(configPanel);
        CEREBRAUI.Add(processorListLabel);
        //CEREBRAUI.Add(labelQuad);
        CEREBRAUI.Add(loadButton);
        CEREBRAUI.Add(mniButton);
        //CEREBRAUI.Add(forwardButton);
        //CEREBRAUI.Add(backButton);
        CEREBRAUI.Add(saveButton);
        CEREBRAUI.Add(layerSlider);
        CEREBRAUI.Add(layerSlidery);
        CEREBRAUI.Add(layerSliderz);
        CEREBRAUI.Add(TransparencySlider);
        CEREBRAUI.Add(reloadButton);
        CEREBRAUI.Add(sliderLabel);
        CEREBRAUI.Add(sliderLabely);
        CEREBRAUI.Add(sliderLabelz);
        CEREBRAUI.Add(exitButton);
        CEREBRAUI.Add(sliderTransparency);

        GameObject go = GameObject.Find("ParentGameObject");
        SimpleUI.Manager mm = go.GetComponent<SimpleUI.Manager>();
        mm.Current = CEREBRAUI;
    }

    void exitButton_onClick(SimpleUI.IUIElement sender, System.EventArgs e)
    {
        Application.Quit();
    }
    void TransparencySlider_onChange(SimpleUI.IUIElement sender, System.EventArgs e)
    {
        transparency = TransparencySlider.hSliderValue;
        //gameObject.SetActive(true);
        //gameObject.renderer.material.color = Color.green;
        GameObject.Find("brain_cover/default/default_MeshPart0").renderer.material.color = new Color(1f, 1f, 1f, (float)transparency / 100);
        GameObject.Find("brain_cover/default/default_MeshPart1").renderer.material.color = new Color(1f, 1f, 1f, (float)transparency / 100);
        GameObject.Find("brain_cover/default/default_MeshPart2").renderer.material.color = new Color(1f, 1f, 1f, (float)transparency / 100);
    }
    void layerSlider_onChange(SimpleUI.IUIElement sender, System.EventArgs e)
    {

        GameObject go = GameObject.Find("ParentGameObject");
        GameObject goTarget = GameObject.Find("TargetGameObject");

        for (int i = goTarget.transform.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyImmediate(goTarget.transform.GetChild(i).gameObject);
        }

        layerValueX = layerSlider.hSliderValue;
        renderPacket(processLayers(layerSlider.hSliderValue));
    }

    
    void layerSlidery_onChange(SimpleUI.IUIElement sender, System.EventArgs e)
    {

        GameObject go = GameObject.Find("ParentGameObject");
        GameObject goTarget = GameObject.Find("TargetGameObject");

        for (int i = goTarget.transform.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyImmediate(goTarget.transform.GetChild(i).gameObject);
        }

        layerValueY = layerSlidery.hSliderValue;
        renderPacket(processLayers(layerSlidery.hSliderValue));
    }
    void layerSliderz_onChange(SimpleUI.IUIElement sender, System.EventArgs e)
    {

        GameObject go = GameObject.Find("ParentGameObject");
        GameObject goTarget = GameObject.Find("TargetGameObject");

        for (int i = goTarget.transform.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyImmediate(goTarget.transform.GetChild(i).gameObject);
        }

        layerValueZ = layerSliderz.hSliderValue;
        renderPacket(processLayers(layerSliderz.hSliderValue));
    }

    void reloadButton_onClick(SimpleUI.IUIElement sender, System.EventArgs e)
    {
        whichQuadrat = 0;
        fileBrowser_onFileSelect(null, lastLoadedData);
    }

    void downButton_onClick(SimpleUI.IUIElement sender, System.EventArgs e)
    {
        int selected = processorListElem.SelectedIndex;

        if (selected < processors.Count - 1)
        {
            string strElem = processorListElem.Data[selected + 1];
            processorListElem.Data[selected + 1] = processorListElem.Data[selected];
            processorListElem.Data[selected] = strElem;

            string[] arrayElem = processorConfigs[selected + 1];
            processorConfigs[selected + 1] = processorConfigs[selected];
            processorConfigs[selected] = arrayElem;

            libsimple.IProcessor procElem = processors[selected + 1];
            processors[selected + 1] = processors[selected];
            processors[selected] = procElem;

            processorListElem.SelectedIndex++;
        }
    }

    void forwardButton_onClick(SimpleUI.IUIElement sender, System.EventArgs e)
    {
        if (whichQuadrat <= System.Math.Ceiling(maxVal/4))
        {
            labelQuad.Text = ((whichQuadrat * jumpCount + 1) + "-" + (whichQuadrat * jumpCount + jumpCount));
            GameObject go = GameObject.Find("ParentGameObject");
            GameObject goTarget = GameObject.Find("TargetGameObject");

            for (int i = goTarget.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(goTarget.transform.GetChild(i).gameObject);
            }
            renderPacket(processSlices());
            whichQuadrat++;
        }
    }

    void backButton_onClick(SimpleUI.IUIElement sender, System.EventArgs e)
    {
        if (whichQuadrat != 0)
        {
            whichQuadrat--;
            labelQuad.Text = ((whichQuadrat * jumpCount + 1) + "-" + (whichQuadrat * jumpCount + jumpCount));
            GameObject go = GameObject.Find("ParentGameObject");
            GameObject goTarget = GameObject.Find("TargetGameObject");

            for (int i = goTarget.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(goTarget.transform.GetChild(i).gameObject);
            }
            renderPacket(processSlices());
        }
    }
    void upButton_onClick(SimpleUI.IUIElement sender, System.EventArgs e)
    {
        int selected = processorListElem.SelectedIndex;

        if (selected > 0)
        {
            string strElem = processorListElem.Data[selected - 1];
            processorListElem.Data[selected - 1] = processorListElem.Data[selected];
            processorListElem.Data[selected] = strElem;

            string[] arrayElem = processorConfigs[selected - 1];
            processorConfigs[selected - 1] = processorConfigs[selected];
            processorConfigs[selected] = arrayElem;

            libsimple.IProcessor procElem = processors[selected - 1];
            processors[selected - 1] = processors[selected];
            processors[selected] = procElem;

            processorListElem.SelectedIndex--;
        }
    }

    void delButton_onClick(SimpleUI.IUIElement sender, System.EventArgs e)
    {
        int selected = processorListElem.SelectedIndex;

        if (selected < processors.Count)
        {
            processors.RemoveAt(selected);
            processorConfigs.RemoveAt(selected);
            processorListElem.Data.RemoveAt(selected);
        }
    }

    //promote to select MNI trans. matrix, and then apply the conversion
    void mniButton_onClick(SimpleUI.IUIElement sender, System.EventArgs e)
    {

        SimpleUI.UIProcessorLister lister = new SimpleUI.UIProcessorLister(new string[] { "MNI Converter" }, new Vector2(0.5f, 0.5f));
        lister.onProcessorCreate += lister_onProcessorCreate;

        CEREBRAUI.Add(lister);


        
    }

    public float moveSpeed = 10f;
    public void OnDoubleClick(float xMouse, float yMouse)
    {
        libsimple.Packet newPacket = potatoPacket.Copy();
        //transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);  
        if (index != -1)
        {
            GameObject go = GameObject.Find("ParentGameObject");
            GameObject goTarget = GameObject.Find("TargetGameObject");

            for (int i = goTarget.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(goTarget.transform.GetChild(i).gameObject);
            }
            //transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            List<libsimple.Packet.Point3D> tmp = new List<libsimple.Packet.Point3D>();
            if (xMouse < Screen.width / 2 && yMouse < Screen.height / 2)
            {

                for (int s = 0; s < newPacket.vXYZ.Length; s++)
                {
                    if (newPacket.vXYZ[s].x < 0 && newPacket.vXYZ[s].y < 0)
                    {
                        tmp.Add(newPacket.vXYZ[s]);
                    }

                }

                newPacket.vXYZ = new libsimple.Packet.Point3D[tmp.Count];
                for (int o = 0; o < tmp.Count; o++)
                {
                    newPacket.vXYZ[o] = tmp[o];
                    newPacket.vXYZ[o].x += 30;
                    newPacket.vXYZ[o].z = 0;
                    newPacket.vXYZ[o].y += 30;
                }
            }
            else if (xMouse > Screen.width / 2 && yMouse < Screen.height / 2)
            {

                for (int s = 0; s < newPacket.vXYZ.Length; s++)
                {
                    if (newPacket.vXYZ[s].x > 0 && newPacket.vXYZ[s].y < 0)
                    {
                        tmp.Add(newPacket.vXYZ[s]);
                    }

                }

                newPacket.vXYZ = new libsimple.Packet.Point3D[tmp.Count];
                for (int o = 0; o < tmp.Count; o++)
                {
                    newPacket.vXYZ[o] = tmp[o];
                    newPacket.vXYZ[o].x -= 30;
                    newPacket.vXYZ[o].z = 0;
                    newPacket.vXYZ[o].y += 30;
                }
            }
            else if (xMouse > Screen.width / 2 && yMouse > Screen.height / 2)
            {

                for (int s = 0; s < newPacket.vXYZ.Length; s++)
                {
                    if (newPacket.vXYZ[s].x > 0 && newPacket.vXYZ[s].y > 0)
                    {
                        tmp.Add(newPacket.vXYZ[s]);
                    }

                }

                newPacket.vXYZ = new libsimple.Packet.Point3D[tmp.Count];
                for (int o = 0; o < tmp.Count; o++)
                {
                    newPacket.vXYZ[o] = tmp[o];
                    newPacket.vXYZ[o].x -= 30;
                    newPacket.vXYZ[o].z = 0;
                    newPacket.vXYZ[o].y -= 30;
                }
            }
            else if (xMouse < Screen.width / 2 && yMouse > Screen.height / 2)
            {

                for (int s = 0; s < newPacket.vXYZ.Length; s++)
                {
                    if (newPacket.vXYZ[s].x < 0 && newPacket.vXYZ[s].y > 0)
                    {
                        tmp.Add(newPacket.vXYZ[s]);
                    }

                }

                newPacket.vXYZ = new libsimple.Packet.Point3D[tmp.Count];
                for (int o = 0; o < tmp.Count; o++)
                {
                    newPacket.vXYZ[o] = tmp[o];
                    newPacket.vXYZ[o].x += 30;
                    newPacket.vXYZ[o].z = 0;
                    newPacket.vXYZ[o].y -= 30;
                }
            }
            renderPacket(newPacket);

        }
        //  o koordinattaki slice i pop up yap(Input.mousePosition.x  ve Input.mousePosition.y kullan)
        //    potatoprocess=true
    }



    void addButton_onClick(SimpleUI.IUIElement sender, System.EventArgs e)
    {
        SimpleUI.UIProcessorLister lister = new SimpleUI.UIProcessorLister(null, new Vector2(0.5f, 0.5f));
        lister.onProcessorCreate += lister_onProcessorCreate;

        CEREBRAUI.Add(lister);
    }

    void lister_onProcessorCreate(string selectedProcessor)
    {
        libsimple.IProcessor proc = libsimple.ProcessorManager.GetProcessorInstance(selectedProcessor);
        SimpleUI.UIProcessorConfigurer configurer = new SimpleUI.UIProcessorConfigurer(proc, new Vector2(0.5f, 0.5f));
        configurer.onProcessorConfigured += configurer_onProcessorConfigured;

        CEREBRAUI.Add(configurer);
    }

    void configurer_onProcessorConfigured(libsimple.IProcessor proc, string[] args)
    {
        processorListElem.Data.Add(proc.GetProcessorName());
        processors.Add(proc);
        processorConfigs.Add(args);
    }

    void processorList_onDoubleClick(SimpleUI.IUIElement sender, object args)
    {
        int selected = processorListElem.SelectedIndex;

        SimpleUI.UIProcessorConfigurer currConfig = new SimpleUI.UIProcessorConfigurer(processors[selected], processorConfigs[selected], new Vector2(0.5f, 0.5f));
        currConfig.onProcessorConfigured += ((p, e) => processorConfigs[selected] = e);

        CEREBRAUI.Add(currConfig);
    }

    void loadButton_onClick(SimpleUI.IUIElement sender, System.EventArgs e)
    {
        whichQuadrat = 0;
        SimpleUI.UIFileBrowser fileBrowser = new SimpleUI.UIFileBrowser(new Vector2(0.5f, 0.5f));
        fileBrowser.onFileSelect += fileBrowser_onFileSelect;
        CEREBRAUI.Add(fileBrowser);
    }

    void saveButton_onClick(SimpleUI.IUIElement sender, System.EventArgs e)
    {

    }
    //decide the rank vaues for each voxel.
    void registerLayers_onLoad(libsimple.Packet p)
    {
        int[] ranks = new int[p.vXYZ.Length];
        int[] ranksY = new int[p.vXYZ.Length];
        int[] ranksZ = new int[p.vXYZ.Length];
        float minX, maxX, avgX;
        float minY, maxY, avgY;
        float minZ, maxZ, avgZ;
        minX = p.vXYZ[0].x;
        maxX = p.vXYZ[0].x;
        minY = p.vXYZ[0].y;
        maxY = p.vXYZ[0].y;
        minZ = p.vXYZ[0].z;
        maxZ = p.vXYZ[0].z;
        for (int i = 0; i < p.vXYZ.Length; i++) { ranks[i] = -1; ranksY[i] = -1; ranksZ[i] = -1; }
        for (int i = 0; i < p.vXYZ.Length; i++)
        {
            if (p.vXYZ[i].x < minX) minX = p.vXYZ[i].x;
            if (p.vXYZ[i].x > maxX) maxX = p.vXYZ[i].x;

            if (p.vXYZ[i].y < minY) minY = p.vXYZ[i].y;
            if (p.vXYZ[i].y > maxY) maxY = p.vXYZ[i].y;

            if (p.vXYZ[i].z < minZ) minZ = p.vXYZ[i].z;
            if (p.vXYZ[i].z > maxZ) maxZ = p.vXYZ[i].z;
        }

        avgX = (maxX + minX) / 2;
        avgY = (maxY + minY) / 2;
        avgZ = (maxZ + minZ) / 2;
        for (int i = 0; i < p.vXYZ.Length; i++)
        {
            if (p.vXYZ[i].x > avgX)
                ranks[i] = (int)(p.vXYZ[i].x - avgX);
            else
                ranks[i] = (int)(avgX - p.vXYZ[i].x);
            //--y
            if (p.vXYZ[i].y > avgY)
                ranksY[i] = (int)(p.vXYZ[i].y - avgY);
            else
                ranksY[i] = (int)(avgY - p.vXYZ[i].y);
            //--z
            if (p.vXYZ[i].z > avgZ)
                ranksZ[i] = (int)(p.vXYZ[i].z - avgZ);
            else
                ranksZ[i] = (int)(avgZ - p.vXYZ[i].z);
        }

        p.setExtra("layerRanks", ranks);
        p.setExtra("layerRanksY", ranksY);
        p.setExtra("layerRanksZ", ranksZ);
        layerSlider.maxValue = (int)(maxX - avgX);
        layerSlidery.maxValue = (int)(maxY - avgY);
        layerSliderz.maxValue = (int)(maxZ - avgZ);
        layerValueX = 0;// layerSlider.maxValue;
        layerValueY = 0;// layerSlidery.maxValue;
        layerValueZ = 0;// layerSliderz.maxValue;
        //layerSlider.maxValue = 100;
    }

    libsimple.Packet processLayers(int layer)
    {
        if (lastLoadedPacket == null)
            return null;
        else
        {

            int layerValX = layerSlider.maxValue - (int)layerValueX;
            int layerValY = layerSlidery.maxValue - (int)layerValueY;
            int layerValZ = layerSliderz.maxValue - (int)layerValueZ;
            int[] ranks = (int[])lastLoadedPacket.getExtra("layerRanks");
            int[] ranksY = (int[])lastLoadedPacket.getExtra("layerRanksY");
            int[] ranksZ = (int[])lastLoadedPacket.getExtra("layerRanksZ");

            libsimple.Packet newPacket = lastLoadedPacket.Copy();

            int[] keyMap = new int[newPacket.vXYZ.Length];

            List<libsimple.Packet.Point3D> tmp = new List<libsimple.Packet.Point3D>();

            for (int i = 0, j = 0; i < newPacket.vXYZ.Length; i++)
            {

                if (ranks[i] <= layerValX && ranksY[i] <= layerValY && ranksZ[i] <= layerValZ)
                {

                    tmp.Add(newPacket.vXYZ[i]);
                    keyMap[i] = j;
                    j++;
                }
                else
                {
                    keyMap[i] = -1;
                }
            }
            newPacket.vXYZ = new libsimple.Packet.Point3D[tmp.Count];
            for (int o = 0; o < tmp.Count; o++) newPacket.vXYZ[o] = tmp[o];

            if (lastLoadedPacket.Edges != null)
            {

                newPacket.Edges = new KeyValuePair<int, double>[newPacket.Edges.GetLength(0), tmp.Count][];
                for (int i = 0; i < newPacket.Edges.GetLength(0); i++)
                {
                    for (int j = 0, k = 0; j < lastLoadedPacket.Edges.GetLength(1); j++)
                    {
                        if (ranks[j] > layerValX || ranksY[j] > layerValY || ranksZ[j] > layerValZ) continue;
                        List<KeyValuePair<int, double>> tempEdges = new List<KeyValuePair<int, double>>(lastLoadedPacket.Edges[i, j]);
                        tempEdges.RemoveAll(x => (ranks[x.Key] > layerValX));
                        tempEdges.RemoveAll(y => (ranksY[y.Key] > layerValY));
                        tempEdges.RemoveAll(z => (ranksZ[z.Key] > layerValZ));

                        for (int l = 0; l < tempEdges.Count; l++)
                        {
                            tempEdges[l] = new KeyValuePair<int, double>(keyMap[tempEdges[l].Key], tempEdges[l].Value);
                        }

                        newPacket.Edges[i, k] = new KeyValuePair<int, double>[tempEdges.Count];
                        newPacket.Edges[i, k] = tempEdges.ToArray();
                        k++;
                    }
                }
            }
            return newPacket;
        }

    }

    libsimple.Packet processSlices()
    {
        if (lastLoadedPacket == null)
            return null;
        else
        {

            /*int layerValX = layerSlider.maxValue - (int)layerValueX;
            int layerValY = layerSlidery.maxValue - (int)layerValueY;
            int layerValZ = layerSliderz.maxValue - (int)layerValueZ;
            int[] ranks = (int[])lastLoadedPacket.getExtra("layerRanks");
            int[] ranksY = (int[])lastLoadedPacket.getExtra("layerRanksY");
            int[] ranksZ = (int[])lastLoadedPacket.getExtra("layerRanksZ");*/

            int[] ranks = new int[lastLoadedPacket.vXYZ.Length];
            int[] ranksY = new int[lastLoadedPacket.vXYZ.Length];
            int[] ranksZ = new int[lastLoadedPacket.vXYZ.Length];
            float minX, maxX;
            float minY, maxY;
            float minZ, maxZ;
            minX = lastLoadedPacket.vXYZ[0].x;
            maxX = lastLoadedPacket.vXYZ[0].x;
            minY = lastLoadedPacket.vXYZ[0].y;
            maxY = lastLoadedPacket.vXYZ[0].y;
            minZ = lastLoadedPacket.vXYZ[0].z;
            maxZ = lastLoadedPacket.vXYZ[0].z;
            for (int i = 0; i < lastLoadedPacket.vXYZ.Length; i++) { ranks[i] = -1; ranksY[i] = -1; ranksZ[i] = -1; }
            for (int i = 0; i < lastLoadedPacket.vXYZ.Length; i++)
            {
                if (lastLoadedPacket.vXYZ[i].x < minX) minX = lastLoadedPacket.vXYZ[i].x;
                if (lastLoadedPacket.vXYZ[i].x > maxX) maxX = lastLoadedPacket.vXYZ[i].x;

                if (lastLoadedPacket.vXYZ[i].y < minY) minY = lastLoadedPacket.vXYZ[i].y;
                if (lastLoadedPacket.vXYZ[i].y > maxY) maxY = lastLoadedPacket.vXYZ[i].y;

                if (lastLoadedPacket.vXYZ[i].z < minZ) minZ = lastLoadedPacket.vXYZ[i].z;
                if (lastLoadedPacket.vXYZ[i].z > maxZ) maxZ = lastLoadedPacket.vXYZ[i].z;
            }

            /*avgX = (maxX + minX) / 2;
            avgY = (maxY + minY) / 2;
            avgZ = (maxZ + minZ) / 2;*/
            for (int i = 0; i < lastLoadedPacket.vXYZ.Length; i++)
            {
                ranks[i] = (int)(lastLoadedPacket.vXYZ[i].x - minX);
                //--y
                ranksY[i] = (int)(lastLoadedPacket.vXYZ[i].y - minY);

                ranksZ[i] = (int)(lastLoadedPacket.vXYZ[i].z - minZ);
            }


            potatoPacket = lastLoadedPacket.Copy();
            if (sliceAxis.Equals("X"))
            {
                int tempval = -1;
                for(int k=0;k<ranks.Length;k++)
                {
                    if (tempval < ranks[k])
                        tempval = ranks[k];
                }
                maxVal = tempval;
                int[] keyMap = new int[potatoPacket.vXYZ.Length];

                List<libsimple.Packet.Point3D> tmp = new List<libsimple.Packet.Point3D>();
                List<int> xCoord = new List<int>();
                List<int> yCoord = new List<int>();
                for (int i = 0, j = 0; i < potatoPacket.vXYZ.Length; i++)
                {
                    for (int num = whichQuadrat * jumpCount + 1; num <= whichQuadrat * jumpCount + jumpCount; num++)
                    {
                        if (ranks[i] == num)
                        {

                            //newPacket.vXYZ[i].x += (float)10.0;
                            if (num == whichQuadrat * 4 + 1)
                            {
                                xCoord.Add(-30);
                                yCoord.Add(+30);
                            }
                            else if (num == whichQuadrat * 4 + 2)
                            {
                                xCoord.Add(+30);
                                yCoord.Add(+30);
                            }
                            else if (num == whichQuadrat * 4 + 3)
                            {
                                xCoord.Add(-30);
                                yCoord.Add(-30);
                            }
                            else if (num == whichQuadrat * 4 + 4)
                            {
                                xCoord.Add(+30);
                                yCoord.Add(-30);
                            }
                            tmp.Add(potatoPacket.vXYZ[i]);
                            keyMap[i] = j;
                            j++;
                        }

                        else
                        {
                            keyMap[i] = -1;
                        }
                    }
                }

                potatoPacket.vXYZ = new libsimple.Packet.Point3D[tmp.Count];
                for (int o = 0; o < tmp.Count; o++) potatoPacket.vXYZ[o] = tmp[o];
                for (int o = 0; o < tmp.Count; o++)
                {
                    potatoPacket.vXYZ[o].x = potatoPacket.vXYZ[o].z+xCoord[o];
                    potatoPacket.vXYZ[o].y += yCoord[o];
                    potatoPacket.vXYZ[o].z = 60;

                }

            }
            else if (sliceAxis.Equals("Y"))
            {

                int tempval = -1;
                for (int k = 0; k < ranksY.Length; k++)
                {
                    if (tempval < ranksY[k])
                        tempval = ranksY[k];
                }
                maxVal = tempval;
                int[] keyMap = new int[potatoPacket.vXYZ.Length];

                List<libsimple.Packet.Point3D> tmp = new List<libsimple.Packet.Point3D>();
                List<int> xCoord = new List<int>();
                List<int> yCoord = new List<int>();
                for (int i = 0, j = 0; i < potatoPacket.vXYZ.Length; i++)
                {
                    for (int num = whichQuadrat * 4 + 1; num <= whichQuadrat * 4 + 4; num++)
                    {
                        if (ranksY[i] == num)
                        {

                            //newPacket.vXYZ[i].x += (float)10.0;
                            if (num == whichQuadrat * 4 + 1)
                            {
                                xCoord.Add(-30);
                                yCoord.Add(+30);
                            }
                            else if (num == whichQuadrat * 4 + 2)
                            {
                                xCoord.Add(+30);
                                yCoord.Add(+30);
                            }
                            else if (num == whichQuadrat * 4 + 3)
                            {
                                xCoord.Add(-30);
                                yCoord.Add(-30);
                            }
                            else if (num == whichQuadrat * 4 + 4)
                            {
                                xCoord.Add(+30);
                                yCoord.Add(-30);
                            }
                            tmp.Add(potatoPacket.vXYZ[i]);
                            keyMap[i] = j;
                            j++;
                        }

                        else
                        {
                            keyMap[i] = -1;
                        }
                    }
                }

                potatoPacket.vXYZ = new libsimple.Packet.Point3D[tmp.Count];
                for (int o = 0; o < tmp.Count; o++) potatoPacket.vXYZ[o] = tmp[o];
                for (int o = 0; o < tmp.Count; o++)
                {
                    potatoPacket.vXYZ[o].x += xCoord[o];
                    potatoPacket.vXYZ[o].y = yCoord[o] + potatoPacket.vXYZ[o].z;
                    potatoPacket.vXYZ[o].z = 60;
                }

            }
            else if (sliceAxis.Equals("Z"))
            {
                int tempval = -1;
                for (int k = 0; k < ranksZ.Length; k++)
                {
                    if (tempval < ranksZ[k])
                        tempval = ranksZ[k];
                }
                maxVal = tempval;
                int[] keyMap = new int[potatoPacket.vXYZ.Length];

                List<libsimple.Packet.Point3D> tmp = new List<libsimple.Packet.Point3D>();
                List<int> xCoord = new List<int>();
                List<int> yCoord = new List<int>();
                for (int i = 0, j = 0; i < potatoPacket.vXYZ.Length; i++)
                {
                    for (int num = whichQuadrat * 4 + 1; num <= whichQuadrat * 4 + 4; num++)
                    {
                        if (ranksZ[i] == num)
                        {

                            //newPacket.vXYZ[i].x += (float)10.0;
                            if (num == whichQuadrat * 4 + 1)
                            {
                                xCoord.Add(-30);
                                yCoord.Add(+30);
                            }
                            else if (num == whichQuadrat * 4 + 2)
                            {
                                xCoord.Add(+30);
                                yCoord.Add(+30);
                            }
                            else if (num == whichQuadrat * 4 + 3)
                            {
                                xCoord.Add(-30);
                                yCoord.Add(-30);
                            }
                            else if (num == whichQuadrat * 4 + 4)
                            {
                                xCoord.Add(+30);
                                yCoord.Add(-30);
                            }
                            tmp.Add(potatoPacket.vXYZ[i]);
                            keyMap[i] = j;
                            j++;
                        }

                        else
                        {
                            keyMap[i] = -1;
                        }
                    }
                }

                potatoPacket.vXYZ = new libsimple.Packet.Point3D[tmp.Count];
                for (int o = 0; o < tmp.Count; o++) potatoPacket.vXYZ[o] = tmp[o];
                for (int o = 0; o < tmp.Count; o++)
                {
                    potatoPacket.vXYZ[o].x += xCoord[o];
                    potatoPacket.vXYZ[o].y += yCoord[o];
                    potatoPacket.vXYZ[o].z = 60;
                }

            }
            return potatoPacket;

        }
    }

    void fileBrowser_onFileSelect(SimpleUI.IUIElement sender, object args)
    {
        if (args != null)
        {
            string filename = args as string;
            lastLoadedData = filename;

            string currDir = System.IO.Directory.GetCurrentDirectory();
            System.IO.Directory.SetCurrentDirectory("./processors");

            libsimple.IProcessor[] openers = libsimple.ProcessorManager.GetReadersFor(filename);

            System.IO.Directory.SetCurrentDirectory(currDir);

            if (openers.Length == 0)
            {
                CEREBRAUI.Add(new SimpleUI.UIMessageBox("Sorry we couldn't load that data :("));
                return;
            }

            if (openers.Length > 1)
            {
                string[] names = new string[openers.Length];
                for (int i = 0; i < names.Length; i++)
                {
                    names[i] = openers[i].GetProcessorName();
                }
                SimpleUI.UIProcessorLister openersLister = new SimpleUI.UIProcessorLister("We found more than one loaders for the data you selected. Please choose one:", names, new Vector2(0.5f, 0.5f));
                openersLister.onProcessorCreate += ((e) => createProcessorAndLoadData(filename, e));

                CEREBRAUI.Add(openersLister);
            }
            else
            {
                createProcessorAndLoadData(filename, openers[0].GetProcessorName());

            }
        }
    }
    void firstPotato() {
        if (whichQuadrat <= System.Math.Ceiling(maxVal / 4))
        {
            labelQuad.Text = ((whichQuadrat * jumpCount + 1) + "-" + (whichQuadrat * jumpCount + jumpCount));
            GameObject go = GameObject.Find("ParentGameObject");
            GameObject goTarget = GameObject.Find("TargetGameObject");

            for (int i = goTarget.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(goTarget.transform.GetChild(i).gameObject);
            }

            GameObject brain0 = GameObject.Find("brain_cover");
            /*for (int i = brain0.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(brain0.transform.GetChild(i).gameObject);
            }*/
            brain0.SetActive(false);
            
            renderPacket(processSlices());
            whichQuadrat++;
        }
    }
    void createProcessorAndLoadData(string filename, string opener)
    {
        GameObject go = GameObject.Find("ParentGameObject");
        GameObject goTarget = GameObject.Find("TargetGameObject");

        for (int i = goTarget.transform.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.DestroyImmediate(goTarget.transform.GetChild(i).gameObject);
        }

        string currDir = System.IO.Directory.GetCurrentDirectory();
        System.IO.Directory.SetCurrentDirectory("./processors");

        libsimple.Pipeline pp = new libsimple.Pipeline();
        pp.AddProcessor(new string[] { opener, filename });
        index = -1;
        for (int i = 0; i < processors.Count; i++)
        {
            List<string> proc = new List<string>();
            proc.Add(processors[i].GetProcessorName());
            proc.AddRange(processorConfigs[i]);
            if (processors[i].GetProcessorName() == "Potato Print")
            {
                index = i;
                sliceAxis = processorConfigs[i][0];
                CEREBRAUI.Add(forwardButton);
                CEREBRAUI.Add(backButton);
                CEREBRAUI.Add(labelQuad);
                //labelQuad.Text = sliceAxis;
                //continue;
            }
            pp.AddProcessor(proc.ToArray());
        }
       
        lastLoadedPacket = pp.Run();
        registerLayers_onLoad(lastLoadedPacket);
       
        renderPacket(processLayers(layerSlider.hSliderValue));
        if (index == -1)
        {
            CEREBRAUI.Remove(forwardButton);
            CEREBRAUI.Remove(backButton);
            CEREBRAUI.Remove(labelQuad);
        }
        else
        {
            firstPotato();
        }
        System.IO.Directory.SetCurrentDirectory(currDir);

    }

    /*void renderPacket2(libsimple.Packet p)
    {
        OptimizedPacketRenderer renderer;

        if (Camera.allCameras[0].gameObject.GetComponent<OptimizedPacketRenderer>() != null)
            Object.DestroyImmediate(Camera.allCameras[0].gameObject.GetComponent<OptimizedPacketRenderer>());

        renderer = (OptimizedPacketRenderer)Camera.allCameras[0].gameObject.AddComponent(typeof(OptimizedPacketRenderer));
        renderer.packetToRender = p;
        //renderer.ScaleTexture = ScaleTexture;

        while (index != -1)
        {
            if (exit == 1 || renderer.packetToReturn != null)
            {
                index = -1;
                break;
            }
        }
        if (renderer.packetToReturn != null)
        {
            lastLoadedPacket = renderer.packetToReturn;
            renderPacket(lastLoadedPacket);
        }
        else
        {
            renderPacket(lastLoadedPacket);
        }
    }*/
    void renderPacket(libsimple.Packet p)
    {
        OptimizedPacketRenderer renderer;

        if (Camera.allCameras[0].gameObject.GetComponent<OptimizedPacketRenderer>() != null)
            Object.DestroyImmediate(Camera.allCameras[0].gameObject.GetComponent<OptimizedPacketRenderer>());

        renderer = (OptimizedPacketRenderer)Camera.allCameras[0].gameObject.AddComponent(typeof(OptimizedPacketRenderer));
        renderer.titanic = this;
        renderer.packetToRender = p;
        //renderer.ScaleTexture = ScaleTexture;
    }
}
