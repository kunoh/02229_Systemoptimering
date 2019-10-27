
.cfg: Architecture
The architecture has at least one CPU and at least one Core for each CPU.

Cpu           : the CPU element, attributed with an "Id" tag, which is a unique integer. It has children tagged with "Core"
Core          : the Core element, attributed with an "Id" tag which is a unique integer within the parent element. It is also attributed with "MacroTick" tag.


.tsk: Applications
The application has tasks and chains. A task is tagged with "Node":
Id                  : Integer; Unique
Name                : A unique name for the task
WCET                : Worst Case Execution Time of the task; in microseconds
Period              : Period of the task in microseconds
Deadline            : Deadline of the task in microseconds


Task Graph; tagged with "TaskGraph":
Name                : A unique name for the Graph.

A Task Graph has at least one edge which is tagged with "Edge" and attributed with the the "Source" and the "Dest".
Source              : The source task of the edge
Dest            	: The sink task of the edge
Cost                : The cost of communication in microseconds
