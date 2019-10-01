package parser;

import java.io.File;
import java.util.HashMap;
import java.util.Map;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

import parser.task.Edge;
import parser.task.Task;

public class TaskParser {

	private Map<String, Task> tasks;

	public TaskParser() {
		Map<String, Task> tasks = new HashMap<>();
	}

	public Map<String, Task> getTasks() {
		return tasks;
	}

	public void parseXml(String filePath) {
		try {
			File inputFile = new File(filePath);
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

					String id = node.getAttribute("Id");
					String name = node.getAttribute("Name");
					int wcet = Integer.parseInt(node.getAttribute("WCET"));
					int period = Integer.parseInt(node.getAttribute("Period"));
					int deadline = Integer.parseInt(node.getAttribute("Deadline"));

					Task task = new Task(id, name, wcet, period, deadline);
					tasks.put(name, task);

					System.out.println("Id : " + id);
					System.out.println("Name : " + name);
					System.out.println("WCET : " + wcet);
					System.out.println("Period : " + period);
					System.out.println("Deadline : " + deadline);
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

					System.out.println("Name : " + taskGraph.getAttribute("Name"));

					NodeList edgeList = taskGraph.getElementsByTagName("Edge");

					for (int temp2 = 0; temp2 < edgeList.getLength(); temp2++) {
						Node e = edgeList.item(temp2);

						System.out.println("\nCurrent Element :" + e.getNodeName());
						if (n.getNodeType() == Node.ELEMENT_NODE) {
							Element edge = (Element) e;

							String source = edge.getAttribute("Source");
							String dest = edge.getAttribute("Dest");
							int cost = Integer.parseInt(edge.getAttribute("Cost"));

							Edge taskEdge = new Edge(source, tasks.get(dest), cost);

							tasks.get(source).addEdge(taskEdge);

							System.out.println("Source : " + source);
							System.out.println("Dest : " + dest);
							System.out.println("Cost : " + cost);
						}
					}
				}
			}

		} catch (Exception e) {
			e.printStackTrace();
		}

	}
}
