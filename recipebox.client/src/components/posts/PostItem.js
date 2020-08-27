import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import Moment from 'react-moment';
import { connect } from 'react-redux';

const PostItem = ({
	auth,
	post: {
		postId,
		nameOfDish,
		prepTime,
		cookingTime,
		averageRating,
		cuisine,
		comments,
		postPhoto,
		created,
		ratings,
		userId
	}
}) => (
	<div className='post bg-white p-1 my-1'>
		<img src='https://upload.wikimedia.org/wikipedia/commons/thumb/f/ff/Fish_and_chips_blackpool.jpg/1200px-Fish_and_chips_blackpool.jpg' />
		<div className='post description bg-white p-1 my-1'>
			<a href='profile.html'>
				<h4>{nameOfDish}</h4>
			</a>
		</div>
		{/* <div className='recipe-pics'>
			<a href='recipe.html'>
				<div className='zoom'>
					<img className='recipe' src={postPhoto.url} />
				</div>
				<h2>{nameOfDish}</h2>
				<p className='post-date'>{created}</p>
				<a href='post.html'>
					<i class='fas fa-comment' />
					<span className='comment-count'>{comments.length}</span>
				</a>
				<button type='button' className='btn btn-danger'>
					<i className='fas fa-times' />
				</button>
				<ul className='list-inline rating-list'>
					<li>
						<i className='fa fa-star ' title='Rate 5 Stars' />
					</li>
					<li>
						<i className='fa fa-star' title='Rate 4 Stars' />
					</li>
					<li>
						<i className='fa fa-star' title='Rate 3 Stars' />
					</li>
					<li>
						<i className='fa fa-star checked' title='Rate 2 Stars' />
					</li>
					<li>
						<i className='fa fa-star checked' title='Rate 1 Star' />
					</li>
					<small title='Ratings'>&nbsp;</small>
				</ul>
				<small title='Ratings'>({ratings.length})</small>
				<p className='text-dark'>
					<i class='far fa-clock'>{` Prep time: ${prepTime}`}</i>
				</p>
				<p className='text-dark'>
					<i class='far fa-clock'>{` Cooking time: ${cookingTime}`}</i>
				</p>
			</a>
		</div> */}
	</div>
);

PostItem.propTypes = {
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth
});

export default connect(mapStateToProps, {})(PostItem);
