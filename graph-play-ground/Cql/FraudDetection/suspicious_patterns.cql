// Simple example: accounts receiving multiple high-value transactions
MATCH (a:Account)<-[:TO]-(t:Transaction)
WHERE t.amount > 10000
RETURN DISTINCT a.id AS SuspiciousAccount;
