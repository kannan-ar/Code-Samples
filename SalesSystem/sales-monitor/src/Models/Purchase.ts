import { Product } from "./Product";
import { User } from "./User";

export interface Purchase {
    quantity: number;
    product: Product;
    user: User
}