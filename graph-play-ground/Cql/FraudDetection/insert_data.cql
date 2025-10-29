// Users
CREATE (u1:User {id:'U1', name:'Alice'});
CREATE (u2:User {id:'U2', name:'Bob'});
CREATE (u3:User {id:'U3', name:'Charlie'});

// Accounts
CREATE (a1:Account {id:'A1'});
CREATE (a2:Account {id:'A2'});
CREATE (a3:Account {id:'A3'});

// Ownership
CREATE (u1)-[:OWNS]->(a1);
CREATE (u2)-[:OWNS]->(a2);
CREATE (u3)-[:OWNS]->(a3);

// Transactions
CREATE (t1:Transaction {id:'T1', amount:5000, time:datetime()});
CREATE (t2:Transaction {id:'T2', amount:15000, time:datetime()});
CREATE (t3:Transaction {id:'T3', amount:20000, time:datetime()});

// Money Flow
CREATE (a1)-[:SENT]->(t1)-[:TO]->(a2);
CREATE (a2)-[:SENT]->(t2)-[:TO]->(a3);
CREATE (a1)-[:SENT]->(t3)-[:TO]->(a3);
