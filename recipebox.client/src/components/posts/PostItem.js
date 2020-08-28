import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import Moment from 'react-moment';
import { connect } from 'react-redux';
import { Card } from 'react-bootstrap';

// var userImage = !post.postPhoto ? <img src="https://www.pinpng.com/pngs/m/341-3415688_no-avatar-png-transparent-png.png"></img> : <img></img>
// var mainPhoto = post.postPhoto.filter((x) => x.isMain == true);
// console.log(mainPhoto);

const PostItem = ({
	auth,
	post: {
		postId,
		nameOfDish,
		description,
		prepTime,
		cookingTime,
		averageRating,
		cuisine,
		mainPhoto,
		created,
		ratings,
		feeds,
		userId
	}
}) => (
	<div className='card'>
		<Link to='#!'>
			<img
				src={
					mainPhoto ? (
						mainPhoto.url
					) : (
						'https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/No_image_available.svg/600px-No_image_available.svg.png'
					)
				}
				alt=''
				className='card__img'
			/>
		</Link>
		<div className='card__content'>
			<Link to='#!' className='card__link'>
				<h2>{nameOfDish}</h2>
			</Link>
			<p>{description}</p>

			<div className='card__info'>
				<div>
					<i className='fas fa-utensils' />
					{' ' + cuisine}
				</div>
				<div>
					<i className='far fa-clock' /> Prep Time: {prepTime}
				</div>
				<div>
					<i className='fas fa-clock' /> Cooking Time: {cookingTime}
				</div>
				<div>
					<i className='fas fa-user-friends' /> Feeds: {feeds}
				</div>
			</div>

			<div className='card__footer'>
				<div>
					<i className='fas fa-user fa-3x icon-a' />
				</div>
				<div>
					<h3> Nicholas Pape</h3>
				</div>
				<div>
					<h4>
						Posted on <Moment format='DD/MM/YYYY'>{created}</Moment>
					</h4>
				</div>
			</div>
		</div>
	</div>
	// <div className='post bg-white p-1 my-1'>
	// 	<img src='https://upload.wikimedia.org/wikipedia/commons/thumb/f/ff/Fish_and_chips_blackpool.jpg/1200px-Fish_and_chips_blackpool.jpg' />
	// 	<div className='post description bg-white p-1 my-1'>
	// 		<a href='profile.html'>
	// 			<h4>{nameOfDish}</h4>
	// 		</a>
	// 	</div>
	// 	<div className='recipe-pics'>
	// 		<a href='recipe.html'>
	// 			<div className='zoom'>
	// 				<img className='recipe' src={postPhoto.url} />
	// 			</div>
	// 			<h2>{nameOfDish}</h2>
	// 			<p className='post-date'>{created}</p>
	// 			<a href='post.html'>
	// 				<i class='fas fa-comment' />
	// 				<span className='comment-count'>{comments.length}</span>
	// 			</a>
	// 			<button type='button' className='btn btn-danger'>
	// 				<i className='fas fa-times' />
	// 			</button>
	// 			<ul className='list-inline rating-list'>
	// 				<li>
	// 					<i className='fa fa-star ' title='Rate 5 Stars' />
	// 				</li>
	// 				<li>
	// 					<i className='fa fa-star' title='Rate 4 Stars' />
	// 				</li>
	// 				<li>
	// 					<i className='fa fa-star' title='Rate 3 Stars' />
	// 				</li>
	// 				<li>
	// 					<i className='fa fa-star checked' title='Rate 2 Stars' />
	// 				</li>
	// 				<li>
	// 					<i className='fa fa-star checked' title='Rate 1 Star' />
	// 				</li>
	// 				<small title='Ratings'>&nbsp;</small>
	// 			</ul>
	// 			<small title='Ratings'>({ratings.length})</small>
	// 			<p className='text-dark'>
	// 				<i class='far fa-clock'>{` Prep time: ${prepTime}`}</i>
	// 			</p>
	// 			<p className='text-dark'>
	// 				<i class='far fa-clock'>{` Cooking time: ${cookingTime}`}</i>
	// 			</p>
	// 		</a>
	// 	</div>
	// </div>
);

PostItem.propTypes = {
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth
});

export default connect(mapStateToProps, {})(PostItem);
