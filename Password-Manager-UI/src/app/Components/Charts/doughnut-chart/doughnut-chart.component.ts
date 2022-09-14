import { Component, Input, OnInit } from '@angular/core';
import {
  Chart,
  ChartConfiguration,
  ChartItem,
  registerables,
} from 'node_modules/chart.js';

@Component({
  selector: 'app-doughnut-chart',
  templateUrl: './doughnut-chart.component.html',
  styleUrls: ['./doughnut-chart.component.css']
})
export class DoughnutChartComponent implements OnInit {
 //step5
 chartItem: ChartItem = document.getElementById('doughnut-chart') as ChartItem;
 //free style
 chart!: Chart;
 @Input() labelsDoughnut: any;
 @Input() datasetsLabelsDoughnut: any;
 @Input() datasetsbackgroundColorDoughnut: any;
 @Input() datasetsdataDoughnut: any;
 @Input() titleText: any;
 listStatus: string[] = ['Weak', 'Safe', 'Reused'];
 safe: number = 5;
 weak: number = 3;
 reused: number = 7;

 constructor() {}

 ngOnInit(): void {
  //  this.opportunityService.getOpportunitys().subscribe((value) => {
  //    value.forEach((v) => {
  //      if (v.statuscode == 1) {
  //        this.open += 1;
  //      } else if (v.statuscode == 3) {
  //        this.won += 1;
  //      } else if (v.statuscode == 4) {
  //        this.close += 1;
  //      }
  //    });
  //    this.createChart();
  //  });
  this.createChart();
 }

 createChart(): void {
   //step1
   Chart.register(...registerables);
   //step2
   const data = {
     labels: this.listStatus,
     datasets: [
       {
         label: 'Weak/Safe/Reused',
         backgroundColor: ['#DC143C', '#90EE90', '#f6c23e'],
         data: [this.weak, this.safe, this.reused],
       },
     ],
   };
   //step3
  //  const options = {
  //    responsive: true,
  //    plugins: {
  //      title: {
  //        display: true,
  //        text: this.titleText,
  //      },
  //    },
  //  };
   //step4
   const config: ChartConfiguration = {
     type: 'doughnut',
     data: data,
    //  options: options,
   };
   //step6
   this.chart = new Chart('doughnut-chart', config);
 }

 changeCountry() {
   this.chart?.update();
 }

}
