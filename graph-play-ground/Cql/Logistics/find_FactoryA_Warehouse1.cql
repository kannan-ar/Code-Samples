MATCH (f:Factory {name: 'FactoryA'}), (w:Warehouse {name: 'Warehouse1'})
RETURN f.name, w.name, round(point.distance(f.location, w.location) / 1000) AS distance_km



