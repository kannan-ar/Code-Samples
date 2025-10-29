MATCH (c:Customer), (w:Warehouse)
WITH c, w, point.distance(c.location, w.location) AS dist
ORDER BY c.name, dist
WITH c, collect({w: w, dist: dist})[0] AS nearest
WITH c, nearest.w AS closestWarehouse, nearest.dist AS dist
MERGE (c)-[r:CLOSEST_TO]->(closestWarehouse)
  SET r.distance_km = round(dist / 1000.0, 2)
RETURN c, r, closestWarehouse;
