package parser;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import java.io.File;
import java.util.HashMap;

public class CpuParser {

    private HashMap<String, CpuParser> cpuParserHashMap;

    public CpuParser() {
        cpuParserHashMap = new HashMap<String, CpuParser>();
    }

    public HashMap<String, CpuParser> getCpuParserHashMap() {
        return cpuParserHashMap;
    }

    public void setCpuParserHashMap(HashMap<String, CpuParser> cpuParserHashMap) {
        this.cpuParserHashMap = cpuParserHashMap;
    }

    public HashMap<String, CpuParser> parse(String Path) {

        try {
            File inputFile = new File(Path);
            DocumentBuilderFactory dbFactory = DocumentBuilderFactory.newInstance();
            DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
            Document doc = dBuilder.parse(inputFile);
            doc.getDocumentElement().normalize();

            System.out.println("Root element :" + doc.getDocumentElement().getNodeName());
            NodeList cpuList = doc.getElementsByTagName("Cpu");
            System.out.println("----------------------------");

            for (int temp = 0; temp < cpuList.getLength(); temp++) {
                Node cp = cpuList.item(temp);
                System.out.println("\nCurrent Element :" + cp.getNodeName());

                if (cp.getNodeType() == Node.ELEMENT_NODE) {
                    Element cpu = (Element) cp;

                    System.out.println("Name : "
                            + cpu.getAttribute("Name"));

                    NodeList coreList = cpu.getElementsByTagName("Core");

                    for (int temp2 = 0; temp2 < coreList.getLength(); temp2++){
                        Node c = coreList.item(temp2);

                        System.out.println("\nCurrent Element :" + c.getNodeName());
                        if (cp.getNodeType() == Node.ELEMENT_NODE){
                            Element core = (Element) c;
                            System.out.println("ID : "
                                    + core.getAttribute("Id"));
                        }
                    }
                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }

        return cpuParserHashMap;
    }
}
