package com.company;

import java.io.File;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.DocumentBuilder;
import org.w3c.dom.Document;
import org.w3c.dom.NodeList;
import org.w3c.dom.Node;
import org.w3c.dom.Element;

public class Main {

    public static void main(String[] args) {

        try {
            File inputFile = new File("C:\\Users\\Kunoh\\Downloads\\Selected exercise test cases\\Selected exercise test cases\\Case 3\\case3.tsk"); //"C:\\Users\\Kunoh\\Desktop\\test.txt");
            DocumentBuilderFactory dbFactory = DocumentBuilderFactory.newInstance();
            DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
            Document doc = dBuilder.parse(inputFile);
            doc.getDocumentElement().normalize();
            System.out.println("Root element :" + doc.getDocumentElement().getNodeName());
            NodeList nodeList = doc.getElementsByTagName("Node");
            System.out.println("----------------------------");

            for (int temp = 0; temp < nodeList.getLength(); temp++) {
                Node n = nodeList.item(temp);
                System.out.println("\nCurrent Element :" + n.getNodeName());

                if (n.getNodeType() == Node.ELEMENT_NODE) {
                    Element node = (Element) n;
                    System.out.println("Id : "
                            + node.getAttribute("Id"));
                    System.out.println("Name : "
                            + node.getAttribute("Name"));
                    System.out.println("WCET : "
                            + node.getAttribute("WCET"));
                    System.out.println("Period : "
                            + node.getAttribute("Period"));
                    System.out.println("Deadline : "
                            + node.getAttribute("Deadline"));
                }
            }

            System.out.println("Root element :" + doc.getDocumentElement().getNodeName());
            NodeList taskGraphList = doc.getElementsByTagName("TaskGraph");
            System.out.println("----------------------------");

            for (int temp = 0; temp < taskGraphList.getLength(); temp++) {
                Node n = taskGraphList.item(temp);
                System.out.println("\nCurrent Element :" + n.getNodeName());

                if (n.getNodeType() == Node.ELEMENT_NODE) {
                    Element taskGraph = (Element) n;

                    System.out.println("Name : "
                            + taskGraph.getAttribute("Name"));

                    NodeList edgeList = taskGraph.getElementsByTagName("Edge");

                    for (int temp2 = 0; temp2 < edgeList.getLength(); temp2++){
                        Node e = edgeList.item(temp2);

                        System.out.println("\nCurrent Element :" + e.getNodeName());
                        if (n.getNodeType() == Node.ELEMENT_NODE){
                            Element edge = (Element) e;
                            System.out.println("Source : "
                                    + edge.getAttribute("Source"));
                            System.out.println("Dest : "
                                    + edge.getAttribute("Dest"));
                            System.out.println("Cost : "
                                    + edge.getAttribute("Cost"));
                        }
                    }
                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}