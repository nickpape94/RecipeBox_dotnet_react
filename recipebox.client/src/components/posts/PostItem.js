import React, { Fragment } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import Ratings from '../post/Ratings';
import Moment from 'react-moment';
import { resetProfilePagination } from '../../actions/user';
import { connect } from 'react-redux';
import { getPost } from '../../actions/post';

// var userImage = !post.postPhoto ? <img src="https://www.pinpng.com/pngs/m/341-3415688_no-avatar-png-transparent-png.png"></img> : <img></img>
// var mainPhoto = post.postPhoto.filter((x) => x.isMain == true);
// console.log(mainPhoto);

const PostItem = ({
	post: {
		postId,
		nameOfDish,
		description,
		prepTime,
		cookingTime,
		averageRating,
		cuisine,
		author,
		userPhotoUrl,
		mainPhoto,
		created,
		ratings,
		feeds,
		userId
	},
	resetProfilePagination,
	postsFromProfile = false,
	favouritesFromProfile = false,
	postsFromCuisine = false
}) => (
	<div className='card'>
		<Link
			to={{
				pathname: `/posts/${postId}`,
				state: {
					postsFromProfile: postsFromProfile,
					favouritesFromProfile: favouritesFromProfile,
					postsFromCuisine: postsFromCuisine
					// cuisine: window.location.href.split('/')[window.location.href.split('/').length - 1]
				}
			}}
		>
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
			<div className='card__header'>
				<Link
					to={{
						pathname: `/posts/${postId}`,
						state: {
							postsFromProfile: postsFromProfile,
							favouritesFromProfile: favouritesFromProfile,
							postsFromCuisine: postsFromCuisine
							// cuisine: window.location.href.split('/')[window.location.href.split('/').length - 1]
						}
					}}
					className='card__link'
				>
					<h2>{nameOfDish}</h2>
				</Link>
				<Ratings averageRating={averageRating} ratings={ratings} />
			</div>

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
				<Link to={`/users/${userId}`} onClick={() => resetProfilePagination()}>
					<div>
						{userPhotoUrl ? (
							<img className='icon-b' src={userPhotoUrl} />
						) : (
							<i className='fas fa-user fa-3x icon-a' />
						)}
					</div>
					<h3> {author}</h3>
				</Link>

				<div>
					<h4>
						Posted on: <Moment format='DD/MM/YYYY'>{created}</Moment>
					</h4>
				</div>
			</div>
		</div>
	</div>
);

PostItem.propTypes = {
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired,
	resetProfilePagination: PropTypes.func.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth
});

export default connect(mapStateToProps, { getPost, resetProfilePagination })(PostItem);
