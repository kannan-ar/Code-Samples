MATCH (f:Factory {name: 'FactoryA'}), (c:Customer {name: 'CustomerY'})
MATCH path = shortestPath(
  (f)-[:CLOSEST_TO|CONNECTS_TO*1..3]-(c)
)
RETURN path;