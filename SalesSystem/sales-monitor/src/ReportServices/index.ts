import { IReport } from "./IReport"
import { Top10Products } from "./Top10Products"

const ReportServices: ReadonlyArray<IReport> = [
    new Top10Products
]

export default ReportServices