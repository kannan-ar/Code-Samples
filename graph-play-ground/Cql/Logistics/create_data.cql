CREATE (f1:Factory {name:'FactoryA', location: point({latitude: 12.97, longitude: 77.59})}),
       (f2:Factory {name:'FactoryB', location: point({latitude: 28.61, longitude: 77.20})});
CREATE (w1:Warehouse {name:'Warehouse1', location: point({latitude: 19.07, longitude: 72.87})}),
       (w2:Warehouse {name:'Warehouse2', location: point({latitude: 22.57, longitude: 88.36})});
CREATE (c1:Customer {name:'CustomerX', location: point({latitude: 13.08, longitude: 80.27})}),
       (c2:Customer {name:'CustomerY', location: point({latitude: 17.38, longitude: 78.49})}),
       (c3:Customer {name:'CustomerZ', location: point({latitude: 26.91, longitude: 75.79})});
