/* eslint-disable @angular-eslint/directive-selector */
import {
  Directive,
  ElementRef,
  Input,
  OnChanges,
  Renderer2,
  SimpleChanges,
} from '@angular/core';

@Directive({
  selector: '[imageLoader]',
  standalone: true,
})
export class ImageLoaderDirective implements OnChanges {
  @Input({ required: true }) src!: string;
  @Input() placeholderSrc = '../../assets/images/placeholder.png';

  constructor(private imageRef: ElementRef, private renderer: Renderer2) {}

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  ngOnChanges(changes: SimpleChanges): void {
    this.loadImage();
  }

  private loadImage() {
    this.renderer.addClass(this.imageRef.nativeElement, 'img-loading');
    const image = new Image();
    if (!this.src) return;

    image.onload = () => {
      this.renderer.removeClass(this.imageRef.nativeElement, 'img-loading');
      this.renderer.setAttribute(this.imageRef.nativeElement, 'src', image.src);
    };

    image.onerror = () => {
      this.renderer.removeClass(this.imageRef.nativeElement, 'img-loading');
      this.renderer.setAttribute(
        this.imageRef.nativeElement,
        'src',
        this.placeholderSrc
      );
    };

    image.src = this.src ?? this.placeholderSrc;
  }
}
