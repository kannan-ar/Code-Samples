MATCH (w1:Warehouse), (w2:Warehouse)
WHERE w1 <> w2
WITH w1, w2, round(point.distance(w1.location, w2.location) / 1000.0, 2) AS distance_km
MERGE (w1)-[r:CONNECTS_TO]->(w2)
  SET r.distance_km = distance_km;
