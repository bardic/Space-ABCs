using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
	private enum Alphabet
	{
		toolow=-1,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,toohigh
	}
	
	public Image leftImage;
	public Image middleImage;
	public Image rightImage;
	public Display display;
	public Canvas mainCanvas;
	public float maxScale = 3;
	public float minScale = 0.5f;

	private BookVO book;
	private Alphabet currentLetter;
	private DeviceOrientation orientation;
	private bool isAnimating;
	private bool swipeBlocked;
	
	private Vector3 fp = Vector3.zero;
	private Vector3 lp = Vector3.zero;
	private float dragDistance = 10;

	private Touch startingTouchZero;
	private Touch startingTouchOne;
	private float startingScale;
	private bool pinchBegan;

	void Start ()
	{
		orientation = Input.deviceOrientation;
		UpdateImageSizes();	
		
		book = ImageService.Instance.LoadImageJSON();
		currentLetter = Alphabet.a;
		Setup(currentLetter);
	}

	private void UpdateImageSizes()
	{
		Vector2 newSize = new Vector2(
			mainCanvas.GetComponent<RectTransform>().rect.width,
			mainCanvas.GetComponent<RectTransform>().rect.width
		);

		leftImage.GetComponent<RectTransform>().sizeDelta = newSize; 
		leftImage.GetComponent<RectTransform>().localPosition = 
			new Vector3(
				newSize.x*-1,
				1,
				0				
			);
		
		middleImage.GetComponent<RectTransform>().sizeDelta = newSize; 
		rightImage.GetComponent<RectTransform>().sizeDelta = newSize; 
		
		rightImage.GetComponent<RectTransform>().localPosition = 
			new Vector3(
				newSize.x,
				1,
				0				
			);
	}
	
	private void OnSwipe(bool rightSwipe)
	{
		if (isAnimating || swipeBlocked) return;
		
		middleImage.transform.DOScale(new Vector3(1, 1, 1), 0.25f);
		
		isAnimating = true;
		bool movingRight = true;
		
		float offset = leftImage.transform.position.x;
		Image otherImageToAnimate = rightImage;
		Alphabet newCurrentLetter = currentLetter + 1  == Alphabet.toohigh ? Alphabet.a : currentLetter+1;
		if (rightSwipe)
		{
			otherImageToAnimate = leftImage;
			offset = rightImage.transform.position.x;
			movingRight = false;
			newCurrentLetter = currentLetter - 1  == Alphabet.toolow ? Alphabet.z : currentLetter-1;
		}

		currentLetter = newCurrentLetter;
		
		otherImageToAnimate.transform.DOMoveX(movingRight?offset+mainCanvas.GetComponent<RectTransform>().rect.width:offset-mainCanvas.GetComponent<RectTransform>().rect.width, 0.5f);
		middleImage.transform.DOMoveX(offset, 0.5f).OnComplete(() =>
		{
			isAnimating = false;
			
			Image tempLeft;
			Image tempMiddle;
			Image tempRight;
			BookVO.PageVO nextPageToLoad;
			
			if (movingRight)
			{	
				leftImage.transform.position = new Vector3(
					rightImage.transform.position.x + mainCanvas.GetComponent<RectTransform>().rect.width,
					rightImage.transform.position.y,
					rightImage.transform.position.z);

				tempLeft = middleImage;
				tempMiddle = rightImage;
				tempRight = leftImage;	
				
				nextPageToLoad = GetRightPage(currentLetter);
			}
			else
			{
				rightImage.transform.position = new Vector3(
					leftImage.transform.position.x  - mainCanvas.GetComponent<RectTransform>().rect.width,
					leftImage.transform.position.y,
					leftImage.transform.position.z);
				
				tempLeft = rightImage;
				tempMiddle = leftImage;
				tempRight = middleImage;
				
				nextPageToLoad = GetLeftPage(currentLetter);
			}
			
			middleImage = tempMiddle;
			leftImage = tempLeft;
			rightImage = tempRight;
			
			if (movingRight)
			{
				LoadSpriteForImage(nextPageToLoad, rightImage);
			}
			else
			{
				LoadSpriteForImage(nextPageToLoad, leftImage);
			}

			UpdateDisplay(middleImage.GetComponent<DisplayImage>().pageVO);
		});
	}

	private void UnblockSwipe()
	{
		swipeBlocked = false;
	}

	private void Setup(Alphabet letter)
	{
		BookVO.PageVO currentPage = GetCurrentPage(letter);
		UpdateDisplay(currentPage);
		
		LoadSpriteForImage(GetLeftPage(letter), leftImage);
		LoadSpriteForImage(currentPage, middleImage);
		LoadSpriteForImage(GetRightPage(letter), rightImage);
	}

	private BookVO.PageVO GetCurrentPage(Alphabet curreAlphabet)
	{
		BookVO.PageVO[] middleImagePages = Array.FindAll(book.pages, page => page.letter == curreAlphabet.ToString());
		BookVO.PageVO randomImage  = GetRandomImage(middleImagePages);

		return randomImage;	
	}
	
	private BookVO.PageVO GetRightPage(Alphabet letter)
	{
		Alphabet nextLetter = letter + 1  == Alphabet.toohigh ? Alphabet.a : letter+1;
		BookVO.PageVO[] rightImagePages = Array.FindAll(book.pages, page => page.letter == nextLetter.ToString());
		BookVO.PageVO randomImage  = GetRandomImage(rightImagePages);

		return randomImage;
	}
	
	private BookVO.PageVO GetLeftPage(Alphabet letter)
	{
		Alphabet previousLetter = letter - 1  == Alphabet.toolow ? Alphabet.z : letter-1;
		BookVO.PageVO[] leftImagePages = Array.FindAll(book.pages, page => page.letter == previousLetter.ToString());
		BookVO.PageVO randomImage = GetRandomImage(leftImagePages);

		return randomImage;
	}

	private void LoadSpriteForImage(BookVO.PageVO page, Image image)
	{
		image.GetComponent<DisplayImage>().pageVO = page;
		image.sprite = Resources.Load<Sprite>("Images/" + page.image);
	}

	private void UpdateDisplay(BookVO.PageVO page)
	{
		display.UpdateDisplay(page.name, page.phonetic, page.description);
	}

	private BookVO.PageVO GetRandomImage(BookVO.PageVO[] pages)
	{
		int randomIndex = UnityEngine.Random.Range(0, pages.Length);
		BookVO.PageVO currentPage = pages[randomIndex];
		return currentPage;
	}

	private void Update()
	{
		if (Input.deviceOrientation != orientation)
		{
			UpdateImageSizes();
		}

		if (!swipeBlocked)
		{
			if (Input.GetMouseButtonDown(0))
			{
				fp = Input.mousePosition;
				lp = Input.mousePosition;
			}
			else
			{
				if (Input.GetMouseButtonUp(0))
				{
					lp = Input.mousePosition;
					if (Mathf.Abs(lp.x - fp.x) > dragDistance)
					{
						OnSwipe(lp.x > fp.x);
					}
				}
			}
		}

		if (Input.touchCount == 2)
		{
			swipeBlocked = true;
			CancelInvoke("UnblockSwipe");
			if (!pinchBegan)
			{
				pinchBegan = true;
				startingScale = middleImage.transform.localScale.x;
				startingTouchZero = Input.GetTouch(0);
				startingTouchOne = Input.GetTouch(1);
			}
			
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);
			
			float startingMag = (startingTouchZero.position - startingTouchOne.position).magnitude;
			float mag = (touchZero.position - touchOne.position).magnitude;
			float scale = (mag / startingMag) * startingScale;
			
			if (scale > maxScale)
			{
				scale = maxScale;
			}

			if (scale < minScale)
			{
				scale = minScale;
			}
			
			middleImage.transform.localScale = new Vector3(scale, scale, 1);
			Invoke("UnblockSwipe", 0.1f);
		}
		else
		{
			pinchBegan = false;
		}
	}
}
